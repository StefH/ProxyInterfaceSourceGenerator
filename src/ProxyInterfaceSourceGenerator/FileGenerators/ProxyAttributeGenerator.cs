namespace ProxyInterfaceSourceGenerator.FileGenerators
{
    internal class ProxyAttributeGenerator : IFileGenerator
    {
        private const string ClassName = "ProxyAttribute";

        public FileData GenerateFile()
        {
            return new FileData($"{ClassName}.cs", $@"using System;

namespace ProxyInterfaceGenerator
{{
    [AttributeUsage(AttributeTargets.Interface)]
    public class {ClassName} : Attribute
    {{
        public Type Type {{ get; }}
        public bool ProxyAll {{ get; }}

        public {ClassName}(Type type, bool proxyAll = false)
        {{
            Type = type;
            ProxyAll = proxyAll;
        }}
    }}
}}");
        }
    }
}