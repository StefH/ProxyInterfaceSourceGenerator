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
        private readonly Context _context;
        private readonly IDictionary<InterfaceDeclarationSyntax, ProxyData> _candidateInterfaces;

        public PartialInterfacesGenerator(Context context, IDictionary<InterfaceDeclarationSyntax, ProxyData> candidateInterfaces)
        {
            _context = context;
            _candidateInterfaces = candidateInterfaces;
        }

        public IEnumerable<FileData> GenerateFiles()
        {
            foreach (var ci in _candidateInterfaces)
            {
                var symbol = _context.GeneratorExecutionContext.Compilation.GetTypeByMetadataName(ci.Value.TypeName);
                if (symbol is null)
                {
                    throw new Exception($"The type '{ci.Value.TypeName}' is not found.");
                }

                yield return new FileData(
                    $"{ci.Value.InterfaceName}.cs",
                    CreatePartialInterfaceCode(symbol, ci.Value.InterfaceName, ci.Value.ProxyAll)
                );
            }
        }

        private string CreatePartialInterfaceCode(INamedTypeSymbol symbol, string interfaceName, bool proxyAll) => $@"using System;

namespace {symbol.ContainingNamespace}
{{
    public partial interface {interfaceName}
    {{
{GenerateProperties(symbol, proxyAll)}

{GenerateMethods(symbol)}
    }}
}}";

        private string GenerateProperties(INamedTypeSymbol symbol, bool proxyAll)
        {
            var str = new StringBuilder();

            // SimpleProperties
            foreach (var property in MemberHelper.GetPublicProperties(symbol, p => p.Type.IsValueType || p.Type.ToString() == "string"))
            {
                str.AppendLine($"        {property.ToPropertyText()}");
                str.AppendLine();
            }

            // InterfaceProperties
            foreach (var property in MemberHelper.GetPublicProperties(symbol,
                p => !(p.Type.IsValueType || p.Type.ToString() == "string"),
                p => p.Type.TypeKind == TypeKind.Interface)
            )
            {
                str.AppendLine($"        {property.ToPropertyText()}");
                str.AppendLine();
            }

            // ComplexProperties
            var complexFilters = new List<Func<IPropertySymbol, bool>>
            {
                p => !(p.Type.IsValueType || p.Type.ToString() == "string"),
                p => p.Type.TypeKind != TypeKind.Interface
            };

            if (proxyAll)
            {

            }

            foreach (var property in MemberHelper.GetPublicProperties(symbol, complexFilters.ToArray()))
            {
                str.AppendLine($"        {property.ToPropertyText()}");
                str.AppendLine();
            }

            return str.ToString();
        }

        private string GenerateMethods(INamedTypeSymbol symbol)
        {
            var str = new StringBuilder();
            foreach (var method in MemberHelper.GetPublicMethods(symbol))
            {
                str.AppendLine($"        {method.ToMethodTextForInterface()};");
                str.AppendLine();
            }

            return "// Methods"; // str.ToString();
        }
    }
}