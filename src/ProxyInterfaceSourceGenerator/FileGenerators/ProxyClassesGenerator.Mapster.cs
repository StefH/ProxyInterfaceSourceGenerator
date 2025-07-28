using System.Text;
using ProxyInterfaceSourceGenerator.Extensions;
using ProxyInterfaceSourceGenerator.Types;

namespace ProxyInterfaceSourceGenerator.FileGenerators;

internal partial class ProxyClassesGenerator
{
    private string GenerateMapperConfigurationForMapster(string className)
    {
        if (Context.ReplacedTypes.Count == 0)
        {
            return string.Empty;
        }

        var str = new StringBuilder();

        var indirectReplacedTypes = Context.ReplacedTypes.Where(r => !r.Direct).ToArray();
        var directReplacedTypes = Context.ReplacedTypes.Where(r => r.Direct).ToArray();

        if (indirectReplacedTypes.Length > 0)
        {
            str.AppendLine($"        static {className}()");
            str.AppendLine(@"        {");

            foreach (var direct in directReplacedTypes)
            {
                var hash = (direct.ClassType + direct.InterfaceType).GetDeterministicHashCodeAsString();
                var instance = $"instance{hash}";
                var proxy = $"proxy{hash}";

                str.AppendLine($"            Mapster.TypeAdapterConfig<{direct.ClassType}, {direct.InterfaceType}>.NewConfig().ConstructUsing({instance} => new {direct.Proxy}({instance}));");
                str.AppendLine($"            Mapster.TypeAdapterConfig<{direct.InterfaceType}, {direct.ClassType}>.NewConfig().MapWith({proxy} => {proxy}._Instance);");
                str.AppendLine();
            }

            str.AppendLine(@"        }");
        }

        str.AppendLine();

        foreach (var indirect in indirectReplacedTypes)
        {
            str.AppendLine($"        private static {indirect.InterfaceType} MapToInterface({indirect.ClassType} value)");
            str.AppendLine(@"        {");
            str.AppendLine($"            return Mapster.TypeAdapter.Adapt<{indirect.InterfaceType}>(value);");
            str.AppendLine(@"        }");
            str.AppendLine();

            str.AppendLine($"        private static {indirect.ClassType} MapToInstance({indirect.InterfaceType} value)");
            str.AppendLine(@"        {");
            str.AppendLine($"            return Mapster.TypeAdapter.Adapt<{indirect.ClassType}>(value);");
            str.AppendLine(@"        }");
            str.AppendLine();
        }

        foreach (var direct in directReplacedTypes)
        {
            if ((direct.UsedIn & TypeUsedIn.MapToInterface) == TypeUsedIn.MapToInterface)
            {
                str.AppendLine($"        private static {direct.InterfaceType} MapToInterface({direct.ClassType} value)");
                str.AppendLine(@"        {");
                str.AppendLine($"            return new {direct.Proxy}(value);");
                str.AppendLine(@"        }");
                str.AppendLine();
            }

            if ((direct.UsedIn & TypeUsedIn.MapToInstance) == TypeUsedIn.MapToInstance)
            {

                str.AppendLine($"        private static {direct.ClassType} MapToInstance({direct.InterfaceType} value)");
                str.AppendLine(@"        {");
                str.AppendLine($"            return value._Instance;");
                str.AppendLine(@"        }");
                str.AppendLine();
            }
        }

        return str.ToString();
    }
}