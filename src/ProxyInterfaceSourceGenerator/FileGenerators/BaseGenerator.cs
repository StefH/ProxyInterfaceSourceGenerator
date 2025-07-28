using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Builders;
using ProxyInterfaceSourceGenerator.Enums;
using ProxyInterfaceSourceGenerator.Extensions;
using ProxyInterfaceSourceGenerator.Models;
using ProxyInterfaceSourceGenerator.Types;

namespace ProxyInterfaceSourceGenerator.FileGenerators;

internal abstract class BaseGenerator
{
    protected readonly Context Context;
    protected readonly bool SupportsNullable;

    protected BaseGenerator(Context context, bool supportsNullable)
    {
        Context = context;
        SupportsNullable = supportsNullable;
    }

    protected string GetPropertyType(IPropertySymbol property, TypeUsedIn typeUsedIn, out bool isReplaced)
    {
        return GetReplacedTypeAsString(property.Type, typeUsedIn, out isReplaced);
    }

    protected string GetParameterType(IParameterSymbol property, TypeUsedIn typeUsedIn, out bool isReplaced)
    {
        return GetReplacedTypeAsString(property.Type, typeUsedIn, out isReplaced);
    }

    protected bool TryFindProxyDataByTypeName(string type, [NotNullWhen(true)] out ProxyData? proxyData)
    {
        proxyData = Context.Candidates.Values.FirstOrDefault(x => x.FullQualifiedTypeName == type);
        return proxyData != null;
    }

    protected string GetWhereStatementFromMethod(IMethodSymbol method)
    {
        if (!method.IsGenericMethod)
        {
            return string.Empty;
        }

        var list = new List<string>();
        foreach (var typeParameterSymbol in method.TypeParameters)
        {
            if (TryGetWhereConstraints(typeParameterSymbol, false, out var constraint))
            {
                list.Add(constraint.ToString());
            }
        }

        return string.Concat(list);
    }

    protected string ResolveInterfaceNameWithOptionalTypeConstraints(INamedTypeSymbol namedTypeSymbol, string interfaceName)
    {
        if (!namedTypeSymbol.IsGenericType)
        {
            return interfaceName;
        }

        var str = new StringBuilder($"{interfaceName}<{string.Join(", ", namedTypeSymbol.TypeArguments.Select(ta => ta.Name))}>");

        foreach (var typeParameterSymbol in namedTypeSymbol.TypeArguments.OfType<ITypeParameterSymbol>())
        {
            if (TryGetWhereConstraints(typeParameterSymbol, false, out var constraint))
            {
                str.Append(constraint);
            }
        }

        return str.ToString();
    }

    /// <summary>
    /// https://www.codeproject.com/Articles/871704/Roslyn-Code-Analysis-in-Easy-Samples-Part-2
    /// </summary>
    public bool TryGetWhereConstraints(ITypeParameterSymbol typeParameterSymbol, bool replaceIt, [NotNullWhen(true)] out ConstraintInfo? constraint)
    {
        var constraints = new List<string>();
        if (typeParameterSymbol.HasReferenceTypeConstraint)
        {
            constraints.Add("class");
        }

        if (typeParameterSymbol.HasValueTypeConstraint)
        {
            constraints.Add("struct");
        }

        foreach (var namedTypeSymbol in typeParameterSymbol.ConstraintTypes.OfType<INamedTypeSymbol>())
        {
            if (replaceIt)
            {
                constraints.Add(GetReplacedTypeAsString(namedTypeSymbol, TypeUsedIn.Constraint, out _));
            }
            else
            {
                constraints.Add(namedTypeSymbol.GetFullType());
            }
        }

        // The new() constraint must be the last constraint specified.
        if (typeParameterSymbol.HasConstructorConstraint)
        {
            constraints.Add("new()");
        }

        if (constraints.Count > 0)
        {
            constraint = new(typeParameterSymbol.Name, constraints);
            return true;
        }

        constraint = null;
        return false;
    }

    internal readonly SymbolDisplayFormat NullableDisplayFormat = new
    (
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions:
            SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
            SymbolDisplayMiscellaneousOptions.UseSpecialTypes |
            SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
    );

    protected string GetReplacedTypeAsString(ITypeSymbol typeSymbol, TypeUsedIn typeUsedIn, out bool isReplaced)
    {
        isReplaced = false;

        var typeSymbolAsString = typeSymbol.ToFullyQualifiedDisplayString();
        var nullableTypeSymbolAsString = typeSymbol.ToDisplayString(NullableFlowState.None, NullableDisplayFormat);

        if (TryFindProxyDataByTypeName(typeSymbolAsString, out var existing))
        {
            TryAddDirect(typeSymbolAsString, typeUsedIn, existing);

            isReplaced = true;
            return FixTypeForNullable(existing.FullInterfaceName, typeSymbol.NullableAnnotation);
        }

        ITypeSymbol[] typeArguments;
        if (typeSymbol is INamedTypeSymbol namedTypedSymbol1)
        {
            typeArguments = namedTypedSymbol1.TypeArguments.ToArray();
        }
        else if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
        {
            typeArguments = [arrayTypeSymbol.ElementType];
        }
        else
        {
            return FixTypeForNullable(typeSymbolAsString, typeSymbol.NullableAnnotation);
        }

        var elementTypeAsStringToBeModified = nullableTypeSymbolAsString;
        foreach (var typeArgument in typeArguments)
        {
            var typeArgumentAsString = typeArgument.ToFullyQualifiedDisplayString();

            if (TryFindProxyDataByTypeName(typeArgumentAsString, out var existingTypeArgument))
            {
                var original = elementTypeAsStringToBeModified;

                elementTypeAsStringToBeModified = elementTypeAsStringToBeModified.Replace(typeArgumentAsString, existingTypeArgument.FullInterfaceName);

                var foundIndirect = Context.ReplacedTypes.FirstOrDefault(r => !r.Direct && r.ClassType == original);
                if (foundIndirect == null)
                {
                    Context.ReplacedTypes.Add(new
                    (
                        original,
                        elementTypeAsStringToBeModified,
                        typeArgumentAsString,
                        existingTypeArgument.FullInterfaceName,
                        string.Empty,
                        direct: false,
                        typeUsedIn
                    ));

                    TryAddDirect(typeArgumentAsString, typeUsedIn, existingTypeArgument);
                }

                isReplaced = true;
            }
        }

        return FixTypeForNullable(elementTypeAsStringToBeModified, typeSymbol.NullableAnnotation);
    }

    private void TryAddDirect(string typeSymbolAsString, TypeUsedIn typeUsedIn, ProxyData existing)
    {
        var found = Context.ReplacedTypes.FirstOrDefault(r => r.Direct && r.ClassType == typeSymbolAsString);
        if (found == null)
        {
            var proxy = $"global::{existing.NamespaceDot}{existing.ShortMetadataName}Proxy"; // global::ProxyInterfaceSourceGeneratorTests.Source.TimeProviderProxy
            Context.ReplacedTypes.Add(new(typeSymbolAsString, existing.FullInterfaceName, string.Empty, string.Empty, proxy, direct: true, typeUsedIn));
        }
        else
        {
            found.UsedIn |= typeUsedIn;
        }
    }

    protected bool TryGetNamedTypeSymbolByFullName(TypeKind kind, string name, IEnumerable<string> usings, [NotNullWhen(true)] out ClassSymbol? classSymbol)
    {
        classSymbol = default;
        const string globalPrefix = "global::";
        if (name.StartsWith(globalPrefix, StringComparison.Ordinal))
        {
            name = name.Substring(globalPrefix.Length);
        }

        // The GetTypeByMetadataName method returns null if no type matches the full name or if 2 or more types (in different assemblies) match the full name.
        var symbol = Context.GeneratorExecutionContext.Compilation.GetTypeByMetadataName(name);

        if (symbol is not null && symbol.TypeKind == kind)
        {
            classSymbol = new ClassSymbol(symbol, symbol.GetBaseTypes(), symbol.AllInterfaces.ToList());
            return true;
        }

        foreach (var @using in usings)
        {
            symbol = Context.GeneratorExecutionContext.Compilation.GetTypeByMetadataName($"{@using}.{name}");
            if (symbol is not null && symbol.TypeKind == kind)
            {
                classSymbol = new ClassSymbol(symbol, symbol.GetBaseTypes(), symbol.AllInterfaces.ToList());
                return true;
            }
        }

        return false;
    }

    protected IReadOnlyList<string> GetMethodParameters(ImmutableArray<IParameterSymbol> parameterSymbols, TypeUsedIn typeUsedIn, bool includeType)
    {
        var methodParameters = new List<string>();
        foreach (var parameterSymbol in parameterSymbols)
        {
            string? type = null;
            if (includeType)
            {
                if (parameterSymbol.GetTypeEnum() == TypeEnum.Complex)
                {
                    type = GetParameterType(parameterSymbol, typeUsedIn, out _);
                }
                else
                {
                    type = FixTypeForNullable(parameterSymbol.Type.ToFullyQualifiedDisplayString(), parameterSymbol.NullableAnnotation);
                }
            }

            methodParameters.Add(MethodParameterBuilder.Build(parameterSymbol, type));
        }

        return methodParameters;
    }

    protected IReadOnlyList<ProxyData> GetExtendsProxyData(ProxyData proxyData, ClassSymbol targetClassSymbol)
    {
        var extendsProxyClasses = new List<ProxyData>();
        foreach (var baseType in targetClassSymbol.BaseTypes)
        {
            var candidate = Context.Candidates.Values.FirstOrDefault(ci => ci.FullQualifiedTypeName == baseType.ToFullyQualifiedDisplayString());
            if (candidate is not null)
            {
                extendsProxyClasses.Add(candidate);
                break;
            }
        }
        return extendsProxyClasses;
    }

    internal static string FixTypeForNullable(string type, NullableAnnotation nullableAnnotation)
    {
        if (nullableAnnotation == NullableAnnotation.Annotated && !type.EndsWith("?", StringComparison.Ordinal))
        {
            return $"{type}?";
        }
        return type;
    }
}