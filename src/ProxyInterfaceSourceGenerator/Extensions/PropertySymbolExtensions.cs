using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Enums;

namespace ProxyInterfaceSourceGenerator.Extensions
{
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

        public static string ToPropertyTextForClass(this IPropertySymbol property)
        {
            var get = property.GetMethod != null ? $"get => _Instance.{property.GetSanitizedName()}; " : string.Empty;
            var set = property.SetMethod != null ? $"set => _Instance.{property.GetSanitizedName()} = value; " : string.Empty;

            return $"{property.Type} {property.GetSanitizedName()} {{ {get}{set}}}";
        }

        public static string ToPropertyTextForClass(this IPropertySymbol property, string overrideType)
        {
            var get = property.GetMethod != null ? $"get => _mapper.Map<{overrideType}>(_Instance.{property.GetSanitizedName()}); " : string.Empty;
            var set = property.SetMethod != null ? $"set => _Instance.{property.GetSanitizedName()} = _mapper.Map<{property.Type}>(value); " : string.Empty;

            return $"{overrideType} {property.GetSanitizedName()} {{ {get}{set}}}";
        }
    }
}