using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Enums;

namespace ProxyInterfaceSourceGenerator.Extensions;

internal static class PropertySymbolExtensions
{
    public static TypeEnum GetTypeEnum(this IPropertySymbol p) => p.Type.GetTypeEnum();

    public static (string PropertyType, string? PropertyName, string GetSet) ToPropertyDetails(this IPropertySymbol property, string? overrideType = null)
    {
        var get = property.GetMethod != null ? "get; " : string.Empty;
        var set = property.SetMethod != null ? "set; " : string.Empty;

        var type = !string.IsNullOrEmpty(overrideType) ? overrideType : $"{property.Type}";

        return (type!, property.GetSanitizedName(), $"{{ {get}{set}}}");
    }
}