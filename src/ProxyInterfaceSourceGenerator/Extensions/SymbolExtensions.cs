using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Enums;

namespace ProxyInterfaceSourceGenerator.Extensions
{
    internal static class SymbolExtensions
    {
        public static TypeEnum GetTypeEnum(this IPropertySymbol p)
        {
            return GetTypeEnum(p.Type);
        }

        public static TypeEnum GetTypeEnum(this IParameterSymbol p)
        {
            return GetTypeEnum(p.Type);
        }

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

        public static bool IsString(this ITypeSymbol ts)
        {
            return ts.ToString() == "string" || ts.ToString() == "string?";
        }

        public static string ToPropertyText(this IPropertySymbol property, string? overrideType = null)
        {
            var get = property.GetMethod != null ? "get; " : string.Empty;
            var set = property.SetMethod != null ? "set; " : string.Empty;

            var type = !string.IsNullOrEmpty(overrideType) ? overrideType : $"{property.Type}";

            return $"{type} {property.Name} {{ {get}{set}}}";
        }

        public static string ToPropertyTextForClass(this IPropertySymbol property)
        {
            var get = property.GetMethod != null ? $"get => _Instance.{property.Name}; " : string.Empty;
            var set = property.SetMethod != null ? $"set => _Instance.{property.Name} = value; " : string.Empty;

            return $"{property.Type} {property.Name} {{ {get}{set}}}";
        }

        public static string ToPropertyTextForClass(this IPropertySymbol property, string overrideType)
        {
            var get = property.GetMethod != null ? $"get => _mapper.Map<{overrideType}>(_Instance.{property.Name}); " : string.Empty;
            var set = property.SetMethod != null ? $"set => _Instance.{property.Name} = _mapper.Map<{property.Type}>(value); " : string.Empty;

            return $"{overrideType} {property.Name} {{ {get}{set}}}";
        }
    }
}
