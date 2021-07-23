namespace ProxyInterfaceSourceGenerator.FileGenerators
{
    internal class ProxyAttributeGenerator : IFileGenerator
    {
        private const string ClassName = "ProxyAttribute";

        public Data GenerateFile()
        {
            return new Data($"{ClassName}.cs", $@"using System;

namespace ProxyInterfaceGenerator
{{
    [AttributeUsage(AttributeTargets.Interface)]
    public class {ClassName} : Attribute
    {{
        public Type Type {{ get; }}
        public bool ProxyAll {{ get; }}

        public {ClassName}(Type type, bool proxyAll = true)
        {{
            Type = type;
            ProxyAll = proxyAll;
        }}
    }}
}}");
        }
    }
}