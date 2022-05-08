using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Enums;

namespace ProxyInterfaceSourceGenerator.Extensions;

internal static class ParameterSymbolExtensions
{
    // private const string ParameterValueDefault = "default";
    private const string ParameterValueNull = "null";

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

    public static string GetDefaultValue(this IParameterSymbol ps)
    {
        if (!ps.HasExplicitDefaultValue)
        {
            return string.Empty;
        }

        var defaultValue = ps.ExplicitDefaultValue switch
        {
            string stringValue => $"\"{stringValue}\"",

            char charValue => $"'{charValue}'",

            null => ps.Type.IsReferenceType ? ParameterValueNull : $"default({ps.Type})",

            _ => ps.ExplicitDefaultValue.ToString()
        };

        return $" = {defaultValue}";
    }

    public static TypeEnum GetTypeEnum(this IParameterSymbol p) => p.Type.GetTypeEnum();
}