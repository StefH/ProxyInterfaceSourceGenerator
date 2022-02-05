using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Enums;
using ProxyInterfaceSourceGenerator.Models;

namespace ProxyInterfaceSourceGenerator.Extensions;

internal static class PropertySymbolExtensions
{
    public static TypeEnum GetTypeEnum(this IPropertySymbol p) =>
        p.Type.GetTypeEnum();

    public static string ToPropertyText(this IPropertySymbol property, string? overrideType = null)
    {
        var get = property.GetMethod != null ? "get; " : string.Empty;
        var set = property.SetMethod != null ? "set; " : string.Empty;

        var type = !string.IsNullOrEmpty(overrideType) ? overrideType : $"{property.Type}";

        return $"{type} {property.GetSanitizedName()} {{ {get}{set}}}";
    }

    public static string ToPropertyTextForClass(this IPropertySymbol property, ClassSymbol targetClassSymbol)
    {
        string instance = !property.IsStatic ?
            "_Instance" :
            $"{targetClassSymbol.Symbol}";

        var get = property.GetMethod != null ? $"get => {instance}.{property.GetSanitizedName()}; " : string.Empty;
        var set = property.SetMethod != null ? $"set => {instance}.{property.GetSanitizedName()} = value; " : string.Empty;

        return $"{property.Type} {property.GetSanitizedName()} {{ {get}{set}}}";
    }

    public static string ToPropertyTextForClass(this IPropertySymbol property, ClassSymbol targetClassSymbol, string overrideType)
    {
        string instance = !property.IsStatic ?
            "_Instance" :
            $"{targetClassSymbol.Symbol}";

        string overrideOrVirtual = string.Empty;
        if (property.IsOverride)
        {
            overrideOrVirtual = "override ";
        }
        else if (property.IsVirtual)
        {
            overrideOrVirtual = "virtual ";
        }

        var get = property.GetMethod != null ? $"get => _mapper.Map<{overrideType}>({instance}.{property.GetSanitizedName()}); " : string.Empty;
        var set = property.SetMethod != null ? $"set => {instance}.{property.GetSanitizedName()} = _mapper.Map<{property.Type}>(value); " : string.Empty;

        return $"{overrideOrVirtual}{overrideType} {property.GetSanitizedName()} {{ {get}{set}}}";
    }
}