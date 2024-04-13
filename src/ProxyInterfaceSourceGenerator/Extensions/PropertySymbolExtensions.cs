using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProxyInterfaceSourceGenerator.Enums;
using ProxyInterfaceSourceGenerator.FileGenerators;

namespace ProxyInterfaceSourceGenerator.Extensions;

internal static class PropertySymbolExtensions
{
    public static TypeEnum GetTypeEnum(this IPropertySymbol p) => p.Type.GetTypeEnum();

    public static (string PropertyType, string? PropertyName, string GetSet)? ToPropertyDetails(this IPropertySymbol property, string? overrideType = null)
    {
        var getIsPublic = property.GetMethod.IsPublic();
        var setIsPublic = property.SetMethod.IsPublic();

        if (!getIsPublic && !setIsPublic)
        {
            return null;
        }

        var get = getIsPublic ? "get; " : string.Empty;
        var set = setIsPublic ? "set; " : string.Empty;

        var type = !string.IsNullOrEmpty(overrideType) ? overrideType 
            : BaseGenerator.FixType(property.Type.ToDisplayString(NullableFlowState.None, SymbolDisplayFormat.FullyQualifiedFormat), property.NullableAnnotation);

        return (type!, property.GetSanitizedName(), $"{{ {get}{set}}}");
    }
}