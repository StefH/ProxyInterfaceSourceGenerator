using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ProxyInterfaceSourceGenerator.Enums;

namespace ProxyInterfaceSourceGenerator.Extensions;

internal static class ParameterSymbolExtensions
{
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

        string defaultValue;
        if (ps.ExplicitDefaultValue == null)
        {
            if (ps.NullableAnnotation == NullableAnnotation.Annotated)
            {
                // The parameter is defined as Nullable, so always return "null".
                return ParameterValueNull;
            }

            defaultValue = ps.Type.IsReferenceType ?
                ParameterValueNull :   // The parameter is a ReferenceType, so return "null".
                $"default({ps.Type})"; // The parameter is not a ReferenceType, so return "default(T)".
        }
        else
        {
            defaultValue = SymbolDisplay.FormatPrimitive(ps.ExplicitDefaultValue, true, false);
        }

        return $" = {defaultValue}";
    }

    public static TypeEnum GetTypeEnum(this IParameterSymbol p) => p.Type.GetTypeEnum();
}