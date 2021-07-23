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
    internal class PartialInterfacesGenerator : IFilesGenerator
    {
        private readonly GeneratorExecutionContext _context;
        private readonly IDictionary<InterfaceDeclarationSyntax, string> _candidateInterfaces;

        public PartialInterfacesGenerator(GeneratorExecutionContext context, IDictionary<InterfaceDeclarationSyntax, string> candidateInterfaces)
        {
            _context = context;
            _candidateInterfaces = candidateInterfaces;
        }

        public IEnumerable<Data> GenerateFiles()
        {
            foreach (var x in _candidateInterfaces)
            {
                string interfaceName = $"I{x.Value.Split('.').Last()}";
                yield return new Data
                {
                    FileName = $"I{interfaceName}.cs",
                    Text = CreatePartialInterfaceCode(_context.Compilation.GetTypeByMetadataName(x.Value), interfaceName)
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
            foreach (IPropertySymbol property in MemberHelper.GetPublicProperties(symbol, p => p.Type.IsValueType || p.Type.ToString() == "string"))
            {
                str.AppendLine($"        {property.ToCode()}");
                str.AppendLine();
            }

            return str.ToString();
        }

        private string GenerateMethods(INamedTypeSymbol symbol)
        {
            var str = new StringBuilder();
            foreach (IMethodSymbol method in MemberHelper.GetPublicMethods(symbol))
            {
                str.AppendLine($"        {method.ToCode()}");
                str.AppendLine();
            }

            return str.ToString();
        }
    }
}