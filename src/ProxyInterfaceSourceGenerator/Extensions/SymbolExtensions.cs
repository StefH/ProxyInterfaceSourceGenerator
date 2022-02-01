using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ProxyInterfaceSourceGenerator.Extensions;

internal static class SymbolExtensions
{
    public static bool IsKeywordOrReserved(this ISymbol symbol) =>
        SyntaxFacts.GetKeywordKind(symbol.Name) != SyntaxKind.None || SyntaxFacts.GetContextualKeywordKind(symbol.Name) != SyntaxKind.None;

    public static string GetSanitizedName(this ISymbol symbol) =>
        symbol.IsKeywordOrReserved() ? $"@{symbol.Name}" : symbol.Name;
}