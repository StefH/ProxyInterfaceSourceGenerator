using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Text;

namespace ProxyInterfaceSourceGenerator.Extensions;

internal static class SymbolExtensions
{
    private static readonly string[] ExcludedAttributes =
    {
        "System.Runtime.CompilerServices.NullableAttribute",
        "System.Runtime.CompilerServices.NullableContextAttribute",
        "System.Runtime.CompilerServices.AsyncStateMachineAttribute"
    };

    public static IReadOnlyList<string> GetAttributesAsList(this ISymbol symbol)
    {
        return symbol
            .GetAttributes()
            .Where(a => a.AttributeClass.IsPublic() && !ExcludedAttributes.Contains(a.AttributeClass?.ToString(), StringComparer.OrdinalIgnoreCase))
            .Select(a => $"[{a}]")
            .ToArray();
    }

    public static string GetAttributesPrefix(this ISymbol symbol)
    {
        var attributes = symbol.GetAttributesAsList();

        return attributes.Any() ? $"{string.Join(" ", attributes)} " : string.Empty;
    }

    //https://stackoverflow.com/questions/27105909/get-fully-qualified-metadata-name-in-roslyn
    public static string GetFullMetadataName(this ISymbol? s)
    {
        if (s == null || IsRootNamespace(s))
        {
            return string.Empty;
        }

        var sb = new StringBuilder(s.MetadataName);
        var last = s;

        s = s.ContainingSymbol;

        while (!IsRootNamespace(s))
        {
            if (s is ITypeSymbol && last is ITypeSymbol)
            {
                sb.Insert(0, '+');
            }
            else
            {
                sb.Insert(0, '.');
            }

            sb.Insert(0, s.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            s = s.ContainingSymbol;
        }

        return sb.ToString();
    }

    public static string GetSanitizedName(this ISymbol symbol) =>
        symbol.IsKeywordOrReserved() ? $"@{symbol.Name}" : symbol.Name;

    public static bool IsKeywordOrReserved(this ISymbol symbol) =>
        SyntaxFacts.GetKeywordKind(symbol.Name) != SyntaxKind.None || SyntaxFacts.GetContextualKeywordKind(symbol.Name) != SyntaxKind.None;

    public static bool IsPublic(this ISymbol? symbol) =>
        symbol is { DeclaredAccessibility: Accessibility.Public };

    private static bool IsRootNamespace(ISymbol symbol) =>
        symbol is INamespaceSymbol { IsGlobalNamespace: true };
}