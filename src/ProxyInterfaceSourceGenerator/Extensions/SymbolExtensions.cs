using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace ProxyInterfaceSourceGenerator.Extensions
{
    internal static class SymbolExtensions
    {
        public static string ToCode(this IPropertySymbol property)
        {
            string get = property.GetMethod != null ? "get; " : string.Empty;
            string set = property.SetMethod != null ? "set; " : string.Empty;

            return $"{property.Type} {property.Name} {{ {get}{set}}}";
        }

        public static string ToCode(this IMethodSymbol method)
        {
            var parameters = new List<string>();
            foreach (var ps in method.Parameters)
            {
                parameters.Add($"{ps.Type} {ps.Name}");
            }

            return $"{method.ReturnType} {method.Name}({string.Join(", ", parameters)});";
        }
    }
}
