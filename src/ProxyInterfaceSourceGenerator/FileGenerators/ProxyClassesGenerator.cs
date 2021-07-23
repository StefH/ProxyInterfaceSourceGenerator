using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProxyInterfaceSourceGenerator.Extensions;
using ProxyInterfaceSourceGenerator.SyntaxReceiver;
using ProxyInterfaceSourceGenerator.Utils;

namespace ProxyInterfaceSourceGenerator.FileGenerators
{
    internal class ProxyClassesGenerator : IFilesGenerator
    {
        private readonly GeneratorExecutionContext _context;
        private readonly IDictionary<InterfaceDeclarationSyntax, ProxyData> _candidateInterfaces;

        public ProxyClassesGenerator(GeneratorExecutionContext context, IDictionary<InterfaceDeclarationSyntax, ProxyData> candidateInterfaces)
        {
            _context = context;
            _candidateInterfaces = candidateInterfaces;
        }

        public IEnumerable<Data> GenerateFiles()
        {
            foreach (var ci in _candidateInterfaces)
            {
                var symbol = _context.Compilation.GetTypeByMetadataName(ci.Value.TypeName);
                if (symbol is null)
                {
                    throw new Exception($"The type '{ci.Value.TypeName}' is not found.");
                }

                string className = $"{ci.Value.TypeName.Split('.').Last()}";

                yield return new Data
                {
                    FileName = $"{className}Proxy.cs",
                    Text = CreateProxyClassCode(symbol, className)
                };
            }
        }

        private string CreateProxyClassCode(INamedTypeSymbol symbol, string className) => $@"using System;

namespace {symbol.ContainingNamespace}
{{
    public class {className}Proxy : I{className}
    {{
        private {className} _instance;

        public {className}Proxy({className} instance)
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