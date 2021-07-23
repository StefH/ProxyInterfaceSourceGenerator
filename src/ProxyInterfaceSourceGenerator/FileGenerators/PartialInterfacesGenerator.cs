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
    internal class PartialInterfacesGenerator : IFilesGenerator
    {
        private readonly GeneratorExecutionContext _context;
        private readonly IDictionary<InterfaceDeclarationSyntax, ProxyData> _candidateInterfaces;

        public PartialInterfacesGenerator(GeneratorExecutionContext context, IDictionary<InterfaceDeclarationSyntax, ProxyData> candidateInterfaces)
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

                string interfaceName = $"I{ci.Value.TypeName.Split('.').Last()}";

                yield return new Data
                {
                    FileName = $"I{interfaceName}.cs",
                    Text = CreatePartialInterfaceCode(symbol, interfaceName)
                };
            }
        }

        private string CreatePartialInterfaceCode(INamedTypeSymbol symbol, string interfaceName) => $@"using System;

namespace {symbol.ContainingNamespace}
{{
    public partial interface {interfaceName}
    {{
{GenerateSimpleProperties(symbol)}

{GenerateMethods(symbol)}
    }}
}}";

        private string GenerateSimpleProperties(INamedTypeSymbol symbol)
        {
            var str = new StringBuilder();
            foreach (var property in MemberHelper.GetPublicProperties(symbol, p => p.Type.IsValueType || p.Type.ToString() == "string"))
            {
                str.AppendLine($"        {property.ToCode()}");
                str.AppendLine();
            }

            return str.ToString();
        }

        private string GenerateMethods(INamedTypeSymbol symbol)
        {
            var str = new StringBuilder();
            foreach (var method in MemberHelper.GetPublicMethods(symbol))
            {
                str.AppendLine($"        {method.ToCode()};");
                str.AppendLine();
            }

            return str.ToString();
        }
    }
}