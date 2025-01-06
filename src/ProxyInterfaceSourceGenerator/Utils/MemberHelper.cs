using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Models;

namespace ProxyInterfaceSourceGenerator.Utils;

internal static class MemberHelper
{
    private static readonly string[] ExcludedMethods = { "ToString", "GetHashCode" };

    public static IReadOnlyList<IPropertySymbol> GetPublicProperties(
        ClassSymbol classSymbol,
        ProxyData proxyData,
        params Func<IPropertySymbol, bool>[] filters)
    {
        var allFilters = new List<Func<IPropertySymbol, bool>>(filters)
        {
            p => p.Kind == SymbolKind.Property
        };

        return GetPublicMembers(classSymbol, proxyData, allFilters.ToArray()).ToArray();
    }

    public static IReadOnlyList<IMethodSymbol> GetPublicMethods(
        ClassSymbol classSymbol,
        ProxyData proxyData,
        Func<IMethodSymbol, bool>? filter = null)
    {
        filter ??= _ => true;

        return
            GetPublicMembers(
                classSymbol,
                proxyData,
                m => m.Kind == SymbolKind.Method,
                m => m.MethodKind == MethodKind.Ordinary,
                m => !ExcludedMethods.Contains(m.Name),
                filter)
            .ToArray();
    }

    public static IReadOnlyList<IMethodSymbol> GetPublicStaticOperators(
        ClassSymbol classSymbol,
        ProxyData proxyData,
        Func<IMethodSymbol, bool>? filter = null)
    {
        filter ??= _ => true;

        return
            GetPublicMembers(
                    classSymbol,
                    proxyData,
                    m => m.Kind == SymbolKind.Method,
                    m => m.MethodKind == MethodKind.Conversion,
                    m => !ExcludedMethods.Contains(m.Name),
                    filter)
                .ToArray();
    }

    public static IReadOnlyList<IGrouping<ISymbol, IMethodSymbol>> GetPublicEvents(
        ClassSymbol classSymbol,
        ProxyData proxyData,
        Func<IMethodSymbol, bool>? filter = null)
    {
        filter ??= _ => true;

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning disable RS1024 // Compare symbols correctly
        return GetPublicMembers(
                classSymbol,
                proxyData,
                m => m.MethodKind is MethodKind.EventAdd or MethodKind.EventRemove/* || m.MethodKind == MethodKind.EventRaise*/,
                filter)
            .GroupBy(e => e.AssociatedSymbol)
            .ToArray();
#pragma warning restore RS1024 // Compare symbols correctly
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
    }

    // TODO : do we need also to check for "SanitizedName()" here?
    private static IReadOnlyList<T> GetPublicMembers<T>(
        ClassSymbol classSymbol,
        ProxyData proxyData,
        params Func<T, bool>[] filters
    ) where T : ISymbol
    {
        var membersQuery = classSymbol.Symbol.GetMembers()
            .OfType<T>()
            .Where(m => m.DeclaredAccessibility == Accessibility.Public);

        if (proxyData.MembersToIgnore.Length > 0)
        {
            membersQuery = membersQuery.Where(symbol => !proxyData.MembersToIgnore.Any(regex => regex.IsMatch(symbol.Name)));
        }

        foreach (var filter in filters)
        {
            membersQuery = membersQuery.Where(filter);
        }

        var ownMembers = membersQuery.ToList();
        var ownMemberNames = ownMembers.Select(x => x.Name);

        if (!proxyData.ProxyBaseClasses)
        {
            return ownMembers;
        }

        var allMembers = ownMembers.ToList();
        var baseType = classSymbol.Symbol.BaseType;

        while (baseType != null && baseType.SpecialType != SpecialType.System_Object)
        {
            var baseMembers = baseType.GetMembers().OfType<T>()
                .Where(m => m.DeclaredAccessibility == Accessibility.Public)
                .Where(x => !ownMemberNames.Contains(x.Name));

            foreach (var filter in filters)
            {
                baseMembers = baseMembers.Where(filter);
            }

            allMembers.AddRange(baseMembers);

            baseType = baseType.BaseType;
        }

        return allMembers;
    }
}