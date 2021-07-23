namespace ProxyInterfaceSourceGenerator.FileGenerators
{
    internal class ProxyAttributeGenerator : IFileGenerator
    {
        private const string ClassName = "ProxyAttribute";

        public Data GenerateFile()
        {
            return new Data
            {
                FileName = $"{ClassName}.cs",
                Text = $@"using System;

namespace ProxyInterfaceGenerator
{{
    [AttributeUsage(AttributeTargets.Interface)]
    public class {ClassName} : Attribute
    {{
        public Type Type {{ get; }}

        public {ClassName}(Type type)
        {{
            Type = type;
        }}
    }}
}}"
            };
        }
    }
}