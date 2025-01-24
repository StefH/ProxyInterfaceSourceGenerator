using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Enums;
using ProxyInterfaceSourceGenerator.FileGenerators;

namespace ProxyInterfaceSourceGenerator.Extensions;

internal static class PropertySymbolExtensions
{
    public static bool IsNullable(this IPropertySymbol p) => p.Type.NullableAnnotation == NullableAnnotation.Annotated;

    public static TypeEnum GetTypeEnum(this IPropertySymbol p) => p.Type.GetTypeEnum();

    public static (string PropertyType, string? PropertyName, string GetSet)? ToPropertyDetails(this IPropertySymbol property, string? overrideType = null)
    {
        var getIsPublic = property.GetMethod.IsPublic();
        var setIsPublic = property.SetMethod.IsPublic();
        var setIsInitOnly = property.SetMethod.IsInitOnly();

        if (!getIsPublic && !setIsPublic)
        {
            return null;
        }

        var set = setIsInitOnly ? "init; " : "set; ";
        var type = !string.IsNullOrEmpty(overrideType) ? overrideType
            : BaseGenerator.FixTypeForNullable(property.Type.ToFullyQualifiedDisplayString(), property.NullableAnnotation);

        return (type!, property.GetSanitizedName(), $"{{ {getIsPublic.IIf("get; ")}{setIsPublic.IIf(set)}}}");
    }
}