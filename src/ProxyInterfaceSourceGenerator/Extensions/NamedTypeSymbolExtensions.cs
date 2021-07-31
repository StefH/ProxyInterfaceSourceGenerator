using Microsoft.CodeAnalysis;
using System.Linq;
using System.Text;

namespace ProxyInterfaceSourceGenerator.Extensions
{
    internal static class NamedTypeSymbolExtensions
    {
        public static string GetFullTypeString(this INamedTypeSymbol type)
        {
            var str = new StringBuilder(type.Name);

            if (type.TypeArguments.Count() > 0)
            {
                str.Append("<");
                
                bool isFirstIteration = true;
                foreach (INamedTypeSymbol typeArg in type.TypeArguments)
                {
                    if (isFirstIteration)
                    {
                        isFirstIteration = false;
                    }
                    else
                    {
                        str.Append(", ");
                    }

                    str.Append(typeArg.GetFullTypeString());
                }

                str.Append(">");
            }

            return str.ToString();
        }

        public static string ResolveInterfaceName(this INamedTypeSymbol type, string interfaceName)
        {
            return !type.IsGenericType ? interfaceName : $"{interfaceName}<{string.Join(", ", type.TypeArguments.Select(ta => ta.Name))}>";
        }

        public static string ResolveProxyClassName(this INamedTypeSymbol type)
        {
            return !type.IsGenericType ? $"{type.Name}Proxy" : $"{type.Name}Proxy<{string.Join(", ", type.TypeArguments.Select(ta => ta.Name))}>";
        }
    }
}