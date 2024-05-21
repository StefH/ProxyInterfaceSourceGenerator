using System.Text;
using Speckle.ProxyGenerator.Extensions;

namespace Speckle.ProxyGenerator.FileGenerators;

internal partial class ProxyClassesGenerator
{
    private string GenerateMapperConfigurationForMapster()
    {
        var str = new StringBuilder();

        foreach (var replacedType in Context.ReplacedTypes)
        {
            TryFindProxyDataByTypeName(replacedType.Key, out var fullTypeName);
            var classNameProxy =
                $"global::{fullTypeName!.NamespaceDot}{fullTypeName!.ShortMetadataName}Proxy";

            var instance =
                $"instance{(replacedType.Key + replacedType.Value).GetDeterministicHashCodeAsString()}";
            var proxy =
                $"proxy{(replacedType.Value + replacedType.Key).GetDeterministicHashCodeAsString()}";

            str.AppendLine(
                $"            Mapster.TypeAdapterConfig<{replacedType.Key}, {replacedType.Value}>.NewConfig().ConstructUsing({instance} => new {classNameProxy}({instance}));"
            );
            str.AppendLine(
                $"            Mapster.TypeAdapterConfig<{replacedType.Value}, {replacedType.Key}>.NewConfig().MapWith({proxy} => (({classNameProxy}) {proxy})._Instance);"
            );

            str.AppendLine();
        }

        return str.ToString();
    }
}
