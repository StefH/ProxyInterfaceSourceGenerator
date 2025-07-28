using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Enums;
using ProxyInterfaceSourceGenerator.FileGenerators;

namespace ProxyInterfaceSourceGenerator.Extensions;

internal static class PropertySymbolExtensions
{
    public static bool IsNullable(this IPropertySymbol p) => p.Type.NullableAnnotation == NullableAnnotation.Annotated;

    public static TypeEnum GetTypeEnum(this IPropertySymbol p) => p.Type.GetTypeEnum();

    public static (string PropertyType, string? PropertyName)? ToPropertyDetails(this IPropertySymbol property, string? overrideType = null)
    {
        var type = !string.IsNullOrEmpty(overrideType) ? overrideType
            : BaseGenerator.FixTypeForNullable(property.Type.ToFullyQualifiedDisplayString(), property.NullableAnnotation);

        return (type!, property.GetSanitizedName());
    }
}