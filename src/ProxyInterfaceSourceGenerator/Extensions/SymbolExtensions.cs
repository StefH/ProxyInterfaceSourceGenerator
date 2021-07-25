using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Enums;

namespace ProxyInterfaceSourceGenerator.Extensions
{
    internal static class SymbolExtensions
    {
        public static TypeEnum GetTypeEnum(this IPropertySymbol p)
        {
            if (p.Type.IsValueType || p.Type.ToString() == "string")
            {
                return TypeEnum.ValueTypeOrString;
            }
            
            if (p.Type.TypeKind == TypeKind.Interface)
            {
                return TypeEnum.Interface;
            }

            return TypeEnum.Complex;
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

        //public static string ToPropertyTextForClass(this IPropertySymbol property, string overrideType, string className)
        //{
        //    var classNameProxy = $"{className}Proxy";
        //    var get = property.GetMethod != null ? $"get => new {classNameProxy}(_Instance.{property.Name}); " : string.Empty;
        //    var set = property.SetMethod != null ? $"set => _Instance.{property.Name} = (({classNameProxy}) value)._Instance; " : string.Empty;

        //    return $"{overrideType} {property.Name} {{ {get}{set}}}";
        //}

        public static string ToMethodText(this IMethodSymbol method)
        {
            var parameters = new List<string>();
            foreach (var ps in method.Parameters)
            {
                parameters.Add($"{ps.Type} {ps.Name}");
            }

            return $"{method.ReturnType} {method.Name}({string.Join(", ", parameters)})";
        }

        public static string ToMethodTextForClass(this IMethodSymbol method)
        {
            var parameters = new List<string>();
            foreach (var ps in method.Parameters)
            {
                parameters.Add($"{ps.Name}");
            }

            return $"{method.ToMethodText()} => _Instance.{method.Name}({string.Join(", ", parameters)});";
        }
    }
}
