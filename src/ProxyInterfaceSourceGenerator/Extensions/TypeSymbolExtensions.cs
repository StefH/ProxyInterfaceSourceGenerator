using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Enums;

namespace ProxyInterfaceSourceGenerator.Extensions;

internal static class TypeSymbolExtensions
{
    public static TypeEnum GetTypeEnum(this ITypeSymbol ts)
    {
        if (ts.IsValueType || ts.IsString())
        {
            return TypeEnum.ValueTypeOrString;
        }

        if (ts.TypeKind == TypeKind.Interface)
        {
            return TypeEnum.Interface;
        }

        return TypeEnum.Complex;
    }

    public static bool IsString(this ITypeSymbol ts) =>
        ts.ToString().ToLowerInvariant() == "string" || ts.ToString().ToLowerInvariant() == "string?";

    public static string ToFullyQualifiedDisplayString(this ITypeSymbol property) =>
        property.ToDisplayString(NullableFlowState.None, SymbolDisplayFormat.FullyQualifiedFormat);
}