using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Enums;
using ProxyInterfaceSourceGenerator.Extensions;
using ProxyInterfaceSourceGenerator.Models;

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

    protected string GetPropertyType(IPropertySymbol property, out bool isReplaced)
    {
        return GetReplacedType(property.Type, out isReplaced);
    }

    protected string GetParameterType(IParameterSymbol property, out bool isReplaced)
    {
        return GetReplacedType(property.Type, out isReplaced);
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
                constraints.Add(GetReplacedType(namedTypeSymbol, out _));
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

    protected string GetReplacedType(ITypeSymbol typeSymbol, out bool isReplaced)
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
            return existing.FullInterfaceName;
        }

        if (typeSymbol is INamedTypeSymbol namedTypedSymbol)
        {
            var propertyTypeAsStringToBeModified = typeSymbolAsString;
            foreach (var typeArgument in namedTypedSymbol.TypeArguments)
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

            return propertyTypeAsStringToBeModified;
        }

        return typeSymbolAsString;
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

    protected IList<string> GetMethodParameters(ImmutableArray<IParameterSymbol> parameters, bool includeType)
    {
        var methodParameters = new List<string>();
        foreach (var ps in parameters)
        {
            string t = string.Empty;
            if (includeType)
            {
                var type = ps.GetTypeEnum() == TypeEnum.Complex ? GetParameterType(ps, out _) : ps.Type.ToString();
                t = $"{ps.GetParamsPrefix()}{ps.GetRefPrefix()}{type} ";
            }
            methodParameters.Add($"{t}{ps.GetSanitizedName()}{ps.GetDefaultValue()}");
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
}