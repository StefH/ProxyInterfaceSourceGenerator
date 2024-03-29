using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Builders;
using ProxyInterfaceSourceGenerator.Enums;
using ProxyInterfaceSourceGenerator.Extensions;
using ProxyInterfaceSourceGenerator.Models;

namespace ProxyInterfaceSourceGenerator.FileGenerators;

internal abstract class BaseGenerator
{
    private const string Star = "*";

    protected readonly Context Context;
    protected readonly bool SupportsNullable;

    protected BaseGenerator(Context context, bool supportsNullable)
    {
        Context = context;
        SupportsNullable = supportsNullable;
    }

    protected string GetPropertyType(IPropertySymbol property, out bool isReplaced)
    {
        return GetReplacedTypeAsString(property.Type, out isReplaced);
    }

    protected string GetParameterType(IParameterSymbol property, out bool isReplaced)
    {
        return GetReplacedTypeAsString(property.Type, out isReplaced);
    }

    protected bool TryFindProxyDataByTypeName(string type, [NotNullWhen(true)] out ProxyData? proxyData)
    {
        proxyData = Context.Candidates.Values.FirstOrDefault(x => x.FullRawTypeName == type);
        if (proxyData != null)
        {
            return true;
        }

        foreach (var ci in Context.Candidates.Values)
        {
            foreach (var u in ci.Usings)
            {
                if ($"{u}.{ci.FullRawTypeName}" == type)
                {
                    proxyData = ci;
                    return true;
                }
            }
        }

        return false;
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
                constraints.Add(GetReplacedTypeAsString(namedTypeSymbol, out _));
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

        if (constraints.Any())
        {
            constraint = new(typeParameterSymbol.Name, constraints);
            return true;
        }

        constraint = null;
        return false;
    }

    protected string GetReplacedTypeAsString(ITypeSymbol typeSymbol, out bool isReplaced)
    {
        isReplaced = false;

        var typeSymbolAsString = typeSymbol.ToString();

        if (TryFindProxyDataByTypeName(typeSymbolAsString, out var existing))
        {
            if (!Context.ReplacedTypes.ContainsKey(typeSymbolAsString))
            {
                Context.ReplacedTypes.Add(typeSymbolAsString, existing.FullInterfaceName);
            }

            isReplaced = true;
            return FixType(existing.FullInterfaceName);
        }

        ITypeSymbol[] typeArguments;
        if (typeSymbol is INamedTypeSymbol namedTypedSymbol1)
        {
            typeArguments = namedTypedSymbol1.TypeArguments.ToArray();
        }
        else if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
        {
            typeArguments = new[] { arrayTypeSymbol.ElementType };
        }
        else
        {
            return FixType(typeSymbolAsString);
        }

        var propertyTypeAsStringToBeModified = typeSymbolAsString;
        foreach (var typeArgument in typeArguments)
        {
            var typeArgumentAsString = typeArgument.ToString();

            if (TryFindProxyDataByTypeName(typeArgumentAsString, out var existingTypeArgument))
            {
                isReplaced = true;

                if (!Context.ReplacedTypes.ContainsKey(typeArgumentAsString))
                {
                    Context.ReplacedTypes.Add(typeArgumentAsString, existingTypeArgument.FullInterfaceName);
                }

                propertyTypeAsStringToBeModified = propertyTypeAsStringToBeModified.Replace(typeArgumentAsString, existingTypeArgument.FullInterfaceName);
            }
        }

        return FixType(propertyTypeAsStringToBeModified);
    }

    protected bool TryGetNamedTypeSymbolByFullName(TypeKind kind, string name, IEnumerable<string> usings, [NotNullWhen(true)] out ClassSymbol? classSymbol)
    {
        classSymbol = default;

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

    protected IReadOnlyList<string> GetMethodParameters(ImmutableArray<IParameterSymbol> parameterSymbols, bool includeType)
    {
        var methodParameters = new List<string>();
        foreach (var parameterSymbol in parameterSymbols)
        {
            string? type = null;
            if (includeType)
            {
                type = parameterSymbol.GetTypeEnum() == TypeEnum.Complex ? GetParameterType(parameterSymbol, out _) : parameterSymbol.Type.ToString();
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
            var candidate = Context.Candidates.Values.FirstOrDefault(ci => ci.FullRawTypeName == baseType.ToString());
            if (candidate is not null)
            {
                extendsProxyClasses.Add(candidate);
                break;
            }

            // Try to find with usings
            foreach (var @using in proxyData.Usings)
            {
                candidate = Context.Candidates.Values.FirstOrDefault(ci => $"{@using}.{ci.FullRawTypeName}" == baseType.ToString());
                if (candidate is not null)
                {
                    // Update the FullRawTypeName
                    candidate.FullRawTypeName = $"{@using}.{candidate.FullRawTypeName}";

                    extendsProxyClasses.Add(candidate);
                    break;
                }
            }
        }
        return extendsProxyClasses;
    }

    /// <summary>
    /// Issue 54
    /// double[*,*] --> double[,]
    /// </summary>
    protected static string FixType(string type)
    {
        return type.Replace(Star, string.Empty);
    }
}