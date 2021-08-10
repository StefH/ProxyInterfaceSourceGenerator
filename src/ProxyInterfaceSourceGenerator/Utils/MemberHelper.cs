using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProxyInterfaceSourceGenerator.Utils
{
    internal static class MemberHelper
    {
        private static string[] _excludedMethods = new string[] { "ToString", "GetHashCode" };

        public static IEnumerable<IPropertySymbol> GetPublicProperties(INamedTypeSymbol classSymbol, params Func<IPropertySymbol, bool>[] filters)
        {
            var allFilters = new List<Func<IPropertySymbol, bool>>(filters);
            allFilters.Add(p => p.Kind == SymbolKind.Property);

            return GetPublicMembers(classSymbol, allFilters.ToArray());
        }

        public static IEnumerable<IMethodSymbol> GetPublicMethods(INamedTypeSymbol classSymbol, Func<IMethodSymbol, bool>? filter = null)
        {
            if (filter is null)
            {
                filter = _ => true;
            }

            return GetPublicMembers(classSymbol,
                m => m.Kind == SymbolKind.Method,
                m => m.MethodKind == MethodKind.Ordinary,
                m => !_excludedMethods.Contains(m.Name),
                filter);
        }

        public static IEnumerable<IGrouping<ISymbol, IMethodSymbol>> GetPublicEvents(INamedTypeSymbol classSymbol, Func<IMethodSymbol, bool>? filter = null)
        {
            if (filter is null)
            {
                filter = _ => true;
            }

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning disable RS1024 // Compare symbols correctly
            return GetPublicMembers(classSymbol,
                m => m.MethodKind == MethodKind.EventAdd || m.MethodKind == MethodKind.EventRemove/* || m.MethodKind == MethodKind.EventRaise*/,
                filter)
                .GroupBy(e => e.AssociatedSymbol);
#pragma warning restore RS1024 // Compare symbols correctly
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }

        // TODO : do we need also to check for "SanitizedName()" here?
        private static IEnumerable<T> GetPublicMembers<T>(INamedTypeSymbol classSymbol, params Func<T, bool>[] filters) where T : ISymbol
        {
            var membersQuery = classSymbol.GetMembers().OfType<T>()
                .Where(m => m.DeclaredAccessibility == Accessibility.Public);

            foreach (var filter in filters)
            {
                membersQuery = membersQuery.Where(filter);
            }

            var members = membersQuery.ToList();

            var propertyNames = membersQuery.Select(x => x.Name);

            var baseType = classSymbol.BaseType;

            while (baseType != null)
            {
                var baseMembers = baseType.GetMembers().OfType<T>()
                    .Where(m => m.DeclaredAccessibility == Accessibility.Public)
                    .Where(x => !propertyNames.Contains(x.Name));

                foreach (var filter in filters)
                {
                    baseMembers = baseMembers.Where(filter);
                }

                members.AddRange(baseMembers);

                baseType = baseType.BaseType;
            }

            return membersQuery;
        }
    }
}