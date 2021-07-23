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

        public IEnumerable<Data> GenerateFiles()
        {
            foreach (var ci in _candidateInterfaces)
            {
                var symbol = _context.GeneratorExecutionContext.Compilation.GetTypeByMetadataName(ci.Value.TypeName);
                if (symbol is null)
                {
                    throw new Exception($"The type '{ci.Value.TypeName}' is not found.");
                }

                string interfaceName = $"I{ci.Value.TypeName.Split('.').Last()}";

                yield return new Data($"I{interfaceName}.cs", CreatePartialInterfaceCode(symbol, interfaceName, ci.Value.ProxyAll));
            }
        }

        private string CreatePartialInterfaceCode(INamedTypeSymbol symbol, string interfaceName, bool proxyAll) => $@"using System;

namespace {symbol.ContainingNamespace}
{{
    public partial interface {interfaceName}
    {{
{GenerateSimpleProperties(symbol)}

{GenerateInterfaceProperties(symbol)}

{GenerateComplexProperties(symbol, proxyAll)}

{GenerateMethods(symbol)}
    }}
}}";

        private string GenerateSimpleProperties(INamedTypeSymbol symbol)
        {
            var str = new StringBuilder();
            foreach (var property in MemberHelper.GetPublicProperties(symbol, p => p.Type.IsValueType || p.Type.ToString() == "string"))
            {
                str.AppendLine($"        {property.ToPropertyTextForInterface()}");
                str.AppendLine();
            }

            return str.ToString();
        }

        private string GenerateInterfaceProperties(INamedTypeSymbol symbol)
        {
            var str = new StringBuilder();
            foreach (var property in MemberHelper.GetPublicProperties(symbol,
                p => !(p.Type.IsValueType || p.Type.ToString() == "string"),
                p => p.Type.TypeKind == TypeKind.Interface)
            )
            {
                str.AppendLine($"        {property.ToPropertyTextForInterface()}");
                str.AppendLine();
            }

            return "// GenerateInterfaceProperties";// str.ToString();
        }

        private string GenerateComplexProperties(INamedTypeSymbol symbol, bool proxyAll)
        {
            var filters = new List<Func<IPropertySymbol, bool>>
            {
                p => !(p.Type.IsValueType || p.Type.ToString() == "string"),
                p => p.Type.TypeKind != TypeKind.Interface
            };

            if (proxyAll)
            {

            }

            var str = new StringBuilder();
            foreach (var property in MemberHelper.GetPublicProperties(symbol, filters.ToArray()))
            {
                str.AppendLine($"        {property.ToPropertyTextForInterface()}");
                str.AppendLine();
            }

            return "// GenerateComplexProperties";// str.ToString();
        }

        private string GenerateMethods(INamedTypeSymbol symbol)
        {
            var str = new StringBuilder();
            foreach (var method in MemberHelper.GetPublicMethods(symbol))
            {
                str.AppendLine($"        {method.ToMethodTextForInterface()};");
                str.AppendLine();
            }

            return str.ToString();
        }
    }
}