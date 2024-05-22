using Microsoft.CodeAnalysis;

namespace Speckle.ProxyGenerator.Extensions;

internal static class NamedTypeSymbolExtensions
{
    public static List<INamedTypeSymbol> GetBaseTypes(this INamedTypeSymbol? type)
    {
        var types = new List<INamedTypeSymbol>();

        bool me = true;
        while (type != null && type.SpecialType != SpecialType.System_Object)
        {
            if (!me)
            {
                types.Add(type);
            }

            type = type.BaseType;
            me = false;
        }

        return types;
    }

    public static string GetFullType(this INamedTypeSymbol namedTypeSymbol)
    {
        // https://www.codeproject.com/Articles/861548/Roslyn-Code-Analysis-in-Easy-Samples-Part
        //var str = new StringBuilder(namedTypeSymbol.Name);

        //if (namedTypeSymbol.TypeArguments.Count() > 0)
        //{
        //    str.AppendFormat("<{0}>", string.Join(", ", namedTypeSymbol.TypeArguments.OfType<INamedTypeSymbol>().Select(typeArg => typeArg.GetFullType())));
        //}

        return namedTypeSymbol.OriginalDefinition.ToString();
    }

    /// <summary>
    /// See https://stackoverflow.com/questions/24157101/roslyns-gettypebymetadataname-and-generic-types
    /// </summary>
    public static string ResolveProxyClassName(this INamedTypeSymbol namedTypeSymbol)
    {
        return !namedTypeSymbol.IsGenericType
            ? $"{namedTypeSymbol.Name}Proxy"
            : $"{namedTypeSymbol.Name}Proxy<{string.Join(", ", namedTypeSymbol.TypeArguments.Select(ta => ta.Name))}>";
    }

    public static List<INamedTypeSymbol> ResolveImplementedInterfaces(
        this INamedTypeSymbol symbol,
        bool proxyBaseClasses
    )
    {
        // Members implemented by us or base classes should go here.
        var publicMembers = symbol
            .GetMembers()
            .Where(m => m.DeclaredAccessibility == Accessibility.Public)
            .ToList();

        // Direct interfaces, recursive interfaces or base class interfaces should go here.
        var interfaces = new List<INamedTypeSymbol>(symbol.Interfaces);
        var baseType = symbol.BaseType;
        while (
            proxyBaseClasses
            && baseType != null
            && baseType.SpecialType != SpecialType.System_Object
        )
        {
            publicMembers.AddRange(
                baseType.GetMembers().Where(m => m.DeclaredAccessibility == Accessibility.Public)
            );
            interfaces.AddRange(baseType.Interfaces);
            baseType = baseType.BaseType;
        }

        // Filter explicitly implemented interfaces.
        var realizedInterfaces = new List<INamedTypeSymbol>();
        foreach (var @interface in interfaces)
        {
            var isRealized = true;
            var allMembers = @interface.AllInterfaces.Aggregate(
                @interface.GetMembers(),
                (xs, x) => xs.AddRange(x.GetMembers())
            );
            foreach (var member in allMembers)
            {
                var implementation = symbol.FindImplementationForInterfaceMember(member);
                if (!publicMembers.Contains(implementation!))
                {
                    isRealized = false;
                    break;
                }
            }

            if (isRealized)
            {
                realizedInterfaces.Add(@interface);
            }
        }

        return realizedInterfaces;
    }
}
