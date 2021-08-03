using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Enums;

namespace ProxyInterfaceSourceGenerator.Extensions
{
    internal static class ParameterSymbolExtensions
    {
        public static string GetRefPrefix(this IParameterSymbol ps)
        {
            switch (ps.RefKind)
            {
                case RefKind.In:
                    return "in ";

                case RefKind.Out:
                    return "out ";

                case RefKind.Ref:
                    return "ref ";

                default:
                    return string.Empty;
            }
        }

        public static string GetParamsPrefix(this IParameterSymbol ps) =>
            ps.IsParams ? "params " : string.Empty;

        public static string GetDefaultValue(this IParameterSymbol ps) =>
            ps.HasExplicitDefaultValue ? $" = {ps.ExplicitDefaultValue}" : string.Empty;

        public static TypeEnum GetTypeEnum(this IParameterSymbol p) =>
            p.Type.GetTypeEnum();
    }
}