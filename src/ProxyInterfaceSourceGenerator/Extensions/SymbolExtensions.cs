using System.Collections.Generic;
using System.Text;
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

        public static string ToProxyCode(this IPropertySymbol property)
        {
            string get = property.GetMethod != null ? $"get => _instance.{property.Name}; " : string.Empty;
            string set = property.SetMethod != null ? $"set => _instance.{property.Name} = value; " : string.Empty;

            return $"{property.Type} {property.Name} {{ {get}{set}}}";
        }

        public static string ToCode(this IMethodSymbol method)
        {
            var parameters = new List<string>();
            foreach (var ps in method.Parameters)
            {
                parameters.Add($"{ps.Type} {ps.Name}");
            }

            return $"{method.ReturnType} {method.Name}({string.Join(", ", parameters)})";
        }

        public static string ToProxyCode(this IMethodSymbol method)
        {
            var parameters = new List<string>();
            foreach (var ps in method.Parameters)
            {
                parameters.Add($"{ps.Name}");
            }

            return $"{method.ToCode()} => _instance.{method.Name}({string.Join(", ", parameters)});";
        }
    }
}
