using Microsoft.CodeAnalysis;

namespace ProxyInterfaceSourceGenerator.Extensions;

internal static class MethodSymbolExtensions
{
    public static string GetMethodNameWithOptionalTypeParameters(this IMethodSymbol method) =>
        !method.IsGenericMethod ? method.Name : $"{method.Name}<{string.Join(", ", method.TypeParameters.Select(tp => tp.Name))}>";

    //public static string GetWhereStatement(this IMethodSymbol method) =>
    //    !method.IsGenericMethod ? string.Empty : string.Join("", method.TypeParameters.Select(tp => tp.GetWhereConstraints()));
}