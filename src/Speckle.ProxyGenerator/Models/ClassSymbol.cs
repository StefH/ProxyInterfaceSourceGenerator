using Microsoft.CodeAnalysis;

namespace Speckle.ProxyGenerator.Models;

internal record ClassSymbol(INamedTypeSymbol Symbol, List<INamedTypeSymbol> BaseTypes, List<INamedTypeSymbol> Interfaces)
{
    public override string ToString()
    {
        return Symbol.ToDisplayString(NullableFlowState.None, SymbolDisplayFormat.FullyQualifiedFormat);
    }
}