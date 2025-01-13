using System.Text;
using ProxyInterfaceSourceGenerator.Enums;
using ProxyInterfaceSourceGenerator.Extensions;

namespace ProxyInterfaceSourceGenerator.FileGenerators;

internal partial class ProxyClassesGenerator
{
    private string GenerateMapperConfigurationForMapster(string className)
    {
        if (Context.IndirectReplacedTypes.Count == 0)
        {
            return string.Empty;
        }

        var str = new StringBuilder();

        str.AppendLine($"        static {className}()");
        str.AppendLine(@"        {");

        foreach (var replacedType in Context.IndirectReplacedTypes)
        {
            var classNameProxy = GetClassNameProxy(replacedType.Key);

            var instance = $"instance{(replacedType.Key + replacedType.Value).GetDeterministicHashCodeAsString()}";
            var proxy = $"proxy{(replacedType.Value + replacedType.Key).GetDeterministicHashCodeAsString()}";

            str.AppendLine($"            Mapster.TypeAdapterConfig<{replacedType.Key}, {replacedType.Value}>.NewConfig().ConstructUsing({instance} => new {classNameProxy}({instance}));");
            str.AppendLine($"            Mapster.TypeAdapterConfig<{replacedType.Value}, {replacedType.Key}>.NewConfig().MapWith({proxy} => (({classNameProxy}) {proxy})._Instance);");
            str.AppendLine();
        }

        str.AppendLine(@"        }");


        return str.ToString();
    }

    private string GetClassNameProxy(string type)
    {
        if (TryFindProxyDataByTypeName(type, out var fullTypeName))
        {
            return $"{GlobalPrefix}{fullTypeName.NamespaceDot}{fullTypeName.ShortMetadataName}Proxy";
        }

        throw new InvalidOperationException($"Cannot find proxy for {type}");
    }

    private string GetAdaptPropertyGet(ReplaceType replaceType, string propertyType, string propertyTypeOriginal, string instancePropertyName, bool isNullable)
    {
        return replaceType switch
        {
            ReplaceType.Indirect => isNullable
                ? $"get => {instancePropertyName} != null ? Mapster.TypeAdapter.Adapt<{propertyType}>({instancePropertyName}) : null; "
                : $"get => Mapster.TypeAdapter.Adapt<{propertyType}>({instancePropertyName}); ",

            ReplaceType.Direct => isNullable
                ? $"get => {instancePropertyName} != null ? new {GetClassNameProxy(propertyTypeOriginal)}({instancePropertyName}) : null; "
                : $"get => new {GetClassNameProxy(propertyTypeOriginal)}({instancePropertyName}); ",

            _ => throw new ArgumentOutOfRangeException(nameof(replaceType), replaceType, null)
        };
    }

    /*
     *var isNullable = property.IsNullable();

       if (getIsPublic)
       {
           var mapster = $"Mapster.TypeAdapter.Adapt<{type}>({instancePropertyName})";
           get = isNullable ?
               $"get => {instancePropertyName} != null ? {mapster} : null; " :
               $"get => {mapster}; ";
       }

       if (setIsPublic)
       {
           var mapster = $"Mapster.TypeAdapter.Adapt<{property.Type}>(value)";
           set = isNullable ?
               $"set => {instancePropertyName} = value != null ? {mapster} : null; " :
               $"set => {instancePropertyName} = {mapster}; ";
       }*/
}