using Microsoft.CodeAnalysis;

namespace ProxyInterfaceSourceGenerator.Model;

internal record ClassSymbol(INamedTypeSymbol Symbol, List<INamedTypeSymbol> BaseTypes, List<INamedTypeSymbol> Interfaces)
{
    public override string ToString()
    {
        return Symbol.ToString();
    }
}