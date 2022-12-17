using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ProxyInterfaceSourceGenerator.Constants;

namespace ProxyInterfaceSourceGenerator.Extensions;

internal static class SymbolExtensions
{
    private static readonly string[] ExcludedAttributes = { InternalClassNames.NullableAttribute };

    public static string GetAttributesPrefix(this ISymbol symbol)
    {
        var attributes = symbol.GetAttributesAsList();

        return attributes.Any() ? $"{string.Join(" ", attributes)} " : string.Empty;
    }

    public static IReadOnlyList<string> GetAttributesAsList(this ISymbol symbol)
    {
        var aa = symbol
            .GetAttributes()
            .Where(a => a.AttributeClass.IsPublic())
            .Select(a => $"[{a}]")
            .ToArray();

        if (aa.Length > 0)
        {
            int yyy = 9;
        }

        return aa;
    }

    public static bool IsPublic(this ISymbol? symbol) =>
        symbol is { DeclaredAccessibility: Accessibility.Public };

    public static bool IsKeywordOrReserved(this ISymbol symbol) =>
        SyntaxFacts.GetKeywordKind(symbol.Name) != SyntaxKind.None || SyntaxFacts.GetContextualKeywordKind(symbol.Name) != SyntaxKind.None;

    public static string GetSanitizedName(this ISymbol symbol) =>
        symbol.IsKeywordOrReserved() ? $"@{symbol.Name}" : symbol.Name;
}