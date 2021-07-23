using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProxyInterfaceSourceGenerator.Extensions;
using ProxyInterfaceSourceGenerator.FileGenerators;
using ProxyInterfaceSourceGenerator.Utils;

namespace ClassLibrarySourceGen
{
    internal class ProxyClassesGenerator : IFilesGenerator
    {
        private readonly GeneratorExecutionContext _context;
        private readonly IDictionary<InterfaceDeclarationSyntax, string> _candidateInterfaces;

        public ProxyClassesGenerator(GeneratorExecutionContext context, IDictionary<InterfaceDeclarationSyntax, string> candidateInterfaces)
        {
            _context = context;
            _candidateInterfaces = candidateInterfaces;
        }

        public IEnumerable<Data> GenerateFiles()
        {
            foreach (var ci in _candidateInterfaces)
            {
                string interfaceName = $"{ci.Value.Split('.').Last()}";
                yield return new Data
                {
                    FileName = $"{interfaceName}Proxy.cs",
                    Text = CreateProxyClassCode(_context.Compilation.GetTypeByMetadataName(ci.Value), interfaceName)
                };
            }
        }

        private string CreateProxyClassCode(INamedTypeSymbol symbol, string interfaceName) => $@"using System;

namespace {symbol.ContainingNamespace}
{{
    public class {interfaceName}Proxy : I{interfaceName}
    {{
        private {interfaceName} _instance;

        public {interfaceName}Proxy({interfaceName} instance)
        {{
            _instance = instance;
        }}

{GenerateSimpleProperties(symbol)}

{GenerateMethods(symbol)}
    }}
}}";

        private string GenerateSimpleProperties(INamedTypeSymbol symbol)
        {
            var str = new StringBuilder();
            foreach (var property in MemberHelper.GetPublicProperties(symbol, p => p.Type.IsValueType || p.Type.ToString() == "string"))
            {
                str.AppendLine($"        public {property.ToProxyCode()}");
                str.AppendLine();
            }

            return str.ToString();
        }

        private string GenerateMethods(INamedTypeSymbol symbol)
        {
            var str = new StringBuilder();
            foreach (var method in MemberHelper.GetPublicMethods(symbol))
            {
                str.AppendLine($"        public {method.ToProxyCode()}");
                str.AppendLine();
            }

            return str.ToString();
        }
    }
}