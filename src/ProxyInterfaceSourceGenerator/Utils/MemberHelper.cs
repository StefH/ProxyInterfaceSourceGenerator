using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Models;

namespace ProxyInterfaceSourceGenerator.Utils;

internal static class MemberHelper
{
    private static readonly string[] ExcludedMethods = { "ToString", "GetHashCode" };

    public static IEnumerable<IPropertySymbol> GetPublicProperties(
        ClassSymbol classSymbol,
        bool proxyBaseClasses,
        params Func<IPropertySymbol, bool>[] filters)
    {
        var allFilters = new List<Func<IPropertySymbol, bool>>(filters)
        {
            p => p.Kind == SymbolKind.Property
        };

        return GetPublicMembers(classSymbol, proxyBaseClasses, allFilters.ToArray());
    }

    public static IEnumerable<IMethodSymbol> GetPublicMethods(
        ClassSymbol classSymbol,
        bool proxyBaseClasses,
        Func<IMethodSymbol, bool>? filter = null)
    {
        filter ??= _ => true;

        return GetPublicMembers(classSymbol,
            proxyBaseClasses,
            m => m.Kind == SymbolKind.Method,
            m => m.MethodKind == MethodKind.Ordinary,
            m => !ExcludedMethods.Contains(m.Name),
            filter);
    }

    public static IEnumerable<IGrouping<ISymbol, IMethodSymbol>> GetPublicEvents(
        ClassSymbol classSymbol,
        bool proxyBaseClasses,
        Func<IMethodSymbol, bool>? filter = null)
    {
        filter ??= _ => true;

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning disable RS1024 // Compare symbols correctly
        return GetPublicMembers(classSymbol,
                proxyBaseClasses,
                m => m.MethodKind is MethodKind.EventAdd or MethodKind.EventRemove/* || m.MethodKind == MethodKind.EventRaise*/,
                filter)
            .GroupBy(e => e.AssociatedSymbol);
#pragma warning restore RS1024 // Compare symbols correctly
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
    }

    // TODO : do we need also to check for "SanitizedName()" here?
    private static IEnumerable<T> GetPublicMembers<T>(
        ClassSymbol classSymbol,
        bool proxyBaseClasses,
        params Func<T, bool>[] filters) where T : ISymbol
    {
        var membersQuery = classSymbol.Symbol.GetMembers().OfType<T>()
            .Where(m => m.DeclaredAccessibility == Accessibility.Public);

        foreach (var filter in filters)
        {
            membersQuery = membersQuery.Where(filter);
        }

        var ownMembers = membersQuery.ToList();
        var ownPropertyNames = ownMembers.Select(x => x.Name);

        if (!proxyBaseClasses)
        {
            return ownMembers;
        }

        var allMembers = ownMembers;
        var baseType = classSymbol.Symbol.BaseType;

        while (baseType != null && baseType.SpecialType != SpecialType.System_Object)
        {
            var baseMembers = baseType.GetMembers().OfType<T>()
                .Where(m => m.DeclaredAccessibility == Accessibility.Public)
                .Where(x => !ownPropertyNames.Contains(x.Name));

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