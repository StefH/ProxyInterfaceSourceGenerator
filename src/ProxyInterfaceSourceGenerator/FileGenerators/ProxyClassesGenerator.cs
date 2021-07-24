using System;
using System.Collections.Generic;
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
        private readonly Context _context;
        private readonly IDictionary<InterfaceDeclarationSyntax, ProxyData> _candidateInterfaces;

        public ProxyClassesGenerator(Context context, IDictionary<InterfaceDeclarationSyntax, ProxyData> candidateInterfaces)
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
                     $"{ci.Value.ClassName}Proxy.cs",
                     CreateProxyClassCode(symbol, ci.Value.InterfaceName, ci.Value.ClassName, ci.Value.ProxyAll)
                );
            }
        }

        private string CreateProxyClassCode(INamedTypeSymbol symbol, string interfaceName, string className, bool proxyAll) => $@"using System;

namespace {symbol.ContainingNamespace}
{{
    public class {className}Proxy : {interfaceName}
    {{
        private {className} _instance;
{GenerateComplexFields(symbol,proxyAll)}

        public {className}Proxy({className} instance)
        {{
            _instance = instance;
        }}

{GenerateProperties(symbol, proxyAll)}

{GenerateMethods(symbol)}
    }}
}}";
        private string GenerateComplexFields(INamedTypeSymbol symbol, bool proxyAll)
        {
            var str = new StringBuilder();

            foreach (var property in GetComplexProperties(symbol, proxyAll))
            {
                str.AppendLine($"        private {property.Type} _{property.Name};");
            }

            return str.ToString();
        }

        private string GenerateProperties(INamedTypeSymbol symbol, bool proxyAll)
        {
            var str = new StringBuilder();

            // SimpleProperties
            foreach (var property in MemberHelper.GetPublicProperties(symbol, p => p.Type.IsValueType || p.Type.ToString() == "string"))
            {
                str.AppendLine($"        public {property.ToPropertyTextForClass()}");
                str.AppendLine();
            }

            // InterfaceProperties
            foreach (var property in MemberHelper.GetPublicProperties(symbol,
                p => !(p.Type.IsValueType || p.Type.ToString() == "string"),
                p => p.Type.TypeKind == TypeKind.Interface)
            )
            {
                str.AppendLine($"        public {property.ToPropertyTextForClass()}");
                str.AppendLine();
            }

            // ComplexProperties
            foreach (var property in GetComplexProperties(symbol, proxyAll))
            {
                str.AppendLine($"        public {property.ToPropertyTextForClass()}");
                str.AppendLine();
            }

            return str.ToString();
        }

        private IEnumerable<IPropertySymbol> GetComplexProperties(INamedTypeSymbol symbol, bool proxyAll)
        {
            var complexFilters = new List<Func<IPropertySymbol, bool>>
            {
                p => !(p.Type.IsValueType || p.Type.ToString() == "string"),
                p => p.Type.TypeKind != TypeKind.Interface
            };

            if (proxyAll)
            {

            }

            return MemberHelper.GetPublicProperties(symbol, complexFilters.ToArray());
        }

        private string GenerateMethods(INamedTypeSymbol symbol)
        {
            var str = new StringBuilder();
            foreach (var method in MemberHelper.GetPublicMethods(symbol))
            {
                str.AppendLine($"        public {method.ToMethodTextForClass()}");
                str.AppendLine();
            }

            return str.ToString();
        }
    }
}