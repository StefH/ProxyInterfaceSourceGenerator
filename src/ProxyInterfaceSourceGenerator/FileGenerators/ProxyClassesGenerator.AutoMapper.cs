using System.Text;
using ProxyInterfaceSourceGenerator.Extensions;

namespace ProxyInterfaceSourceGenerator.FileGenerators;

internal partial class ProxyClassesGenerator
{
    private static string GeneratePrivateAutoMapper()
    {
        return "        private readonly IMapper _mapper;";
    }

    private string GenerateMapperConfigurationForAutoMapper()
    {
        var str = new StringBuilder();

        str.AppendLine("            _mapper = new MapperConfiguration(cfg =>");
        str.AppendLine("            {");
        foreach (var replacedType in Context.ReplacedTypes)
        {
            TryFindProxyDataByTypeName(replacedType.Key, out var fullTypeName);
            var classNameProxy = $"{fullTypeName!.Namespace}.{fullTypeName.ShortTypeName}Proxy";

            var instance = $"instance{(replacedType.Key + replacedType.Value).GetDeterministicHashCodeAsString()}";
            var proxy = $"proxy{(replacedType.Value + replacedType.Key).GetDeterministicHashCodeAsString()}";

            str.AppendLine($"                cfg.CreateMap<{replacedType.Key}, {replacedType.Value}>().ConstructUsing({instance} => new {classNameProxy}({instance}));");
            str.AppendLine($"                cfg.CreateMap<{replacedType.Value}, {replacedType.Key}>().ConstructUsing({proxy} => (({classNameProxy}) {proxy})._Instance);");
            str.AppendLine();
        }
        str.AppendLine("            }).CreateMapper();");

        return str.ToString();
    }
}