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
    internal class ProxyClassesGenerator : BaseGenerator, IFilesGenerator
    {
        public ProxyClassesGenerator(Context context, IDictionary<InterfaceDeclarationSyntax, ProxyData> candidateInterfaces) :
            base(context, candidateInterfaces)
        {
        }

        public IEnumerable<FileData> GenerateFiles()
        {
            foreach (var ci in _candidateInterfaces)
            {
                var symbol = GetType(ci.Value.TypeName);

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
{GeneratePrivateComplexInterfaceFields(symbol,proxyAll)}

        public {className}Proxy({className} instance)
        {{
            _instance = instance;
        }}

{GeneratePublicProperties(symbol, proxyAll)}

{GeneratePublicMethods(symbol)}
    }}
}}";
        private string GeneratePrivateComplexInterfaceFields(INamedTypeSymbol symbol, bool proxyAll)
        {
            if (!proxyAll)
            {
                return string.Empty;
            }

            var str = new StringBuilder();

            foreach (var property in GetComplexProperties(symbol, proxyAll))
            {
                str.AppendLine($"        private {property.Type} _{property.Name};");
            }

            return str.ToString();
        }

        private string GeneratePublicProperties(INamedTypeSymbol symbol, bool proxyAll)
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

        private string GeneratePublicMethods(INamedTypeSymbol symbol)
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