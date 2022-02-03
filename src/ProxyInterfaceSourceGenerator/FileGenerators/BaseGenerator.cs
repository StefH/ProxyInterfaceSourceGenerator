using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
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

    protected string GetReplacedType(ITypeSymbol typeSymbol, out bool isReplaced)
    {
        isReplaced = false;

        var typeSymbolAsString = typeSymbol.ToString();

        var existing = Context.CandidateInterfaces.Values.FirstOrDefault(x => x.RawTypeName == typeSymbolAsString);
        if (existing is not null)
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
                var exist = Context.CandidateInterfaces.Values.FirstOrDefault(x => x.RawTypeName == typeArgumentAsString);
                if (exist is not null)
                {
                    isReplaced = true;

                    if (!Context.ReplacedTypes.ContainsKey(typeArgumentAsString))
                    {
                        Context.ReplacedTypes.Add(typeArgumentAsString, exist.FullInterfaceName);
                    }

                    propertyTypeAsStringToBeModified = propertyTypeAsStringToBeModified.Replace(typeArgumentAsString, exist.FullInterfaceName);
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
}