using System.Text;
using ProxyInterfaceSourceGenerator.Extensions;

namespace ProxyInterfaceSourceGenerator.FileGenerators;

internal partial class ProxyClassesGenerator
{
    private string GenerateMapperConfigurationForMapster()
    {
        var str = new StringBuilder();

        foreach (var replacedType in Context.ReplacedTypes)
        {
            TryFindProxyDataByTypeName(replacedType.Key, out var fullTypeName);
            var classNameProxy = $"{fullTypeName!.Namespace}.{fullTypeName.ShortTypeName}Proxy";

            var instance = $"instance{(replacedType.Key + replacedType.Value).GetDeterministicHashCodeAsString()}";
            var proxy = $"proxy{(replacedType.Value + replacedType.Key).GetDeterministicHashCodeAsString()}";

            //Mapster.TypeAdapterConfig<Web, IWeb>.NewConfig().ConstructUsing(instance_841809920 => new ProxyInterfaceConsumer.PnP.WebProxy(instance_841809920));
            //Mapster.TypeAdapterConfig<IWeb, Web>.NewConfig().MapWith(proxy1898650104 => ((ProxyInterfaceConsumer.PnP.WebProxy)proxy1898650104)._Instance);

            str.AppendLine($"            Mapster.TypeAdapterConfig<{replacedType.Key}, {replacedType.Value}>.NewConfig().ConstructUsing({instance} => new {classNameProxy}({instance}));");
            str.AppendLine($"            Mapster.TypeAdapterConfig<{replacedType.Value}, {replacedType.Key}>.NewConfig().MapWith({proxy} => (({classNameProxy}) {proxy})._Instance);");
            str.AppendLine();
        }

        return str.ToString();
    }
}