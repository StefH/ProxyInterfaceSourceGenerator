using System.Text;
using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Extensions;

namespace ProxyInterfaceSourceGenerator.Builders;

internal static class MethodParameterBuilder
{
    public static string Build(IParameterSymbol parameterSymbol, string? type, bool supportsNullable)
    {
        var stringBuilder = new StringBuilder();
        if (type is not null)
        {
            stringBuilder.Append(parameterSymbol.GetAttributesPrefix()); // "" or [NotNullWhen(true)]
            stringBuilder.Append(parameterSymbol.GetParamsPrefix()); // "" or "params "
            stringBuilder.Append(parameterSymbol.GetRefKindPrefix()); // "" or "out "
            stringBuilder.AppendFormat("{0} ", type); // string or another type
        }
        
        stringBuilder.Append(parameterSymbol.GetSanitizedName()); // "s" or "i" or ...
        stringBuilder.Append(parameterSymbol.GetDefaultValue(supportsNullable)); // "" or the value

        return stringBuilder.ToString();
    }
}