using System.Text;
using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Extensions;

namespace ProxyInterfaceSourceGenerator.Builders;

internal static class MethodParameterBuilder
{
    public static string Build(IParameterSymbol parameterSymbol, string type)
    {
        var a = parameterSymbol.GetAttributes();

        var stringBuilder = new StringBuilder();
        stringBuilder.Append(parameterSymbol.GetParamsPrefix());
        stringBuilder.Append(parameterSymbol.GetRefPrefix());
        stringBuilder.AppendFormat("{0} ", type);
        stringBuilder.Append(parameterSymbol.GetSanitizedName());
        stringBuilder.Append(parameterSymbol.GetDefaultValue());

        return stringBuilder.ToString();
    }
}