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
    internal class PartialInterfacesGenerator : BaseGenerator, IFilesGenerator
    {
        private readonly List<FileData> files = new List<FileData>();

        public PartialInterfacesGenerator(Context context, IDictionary<InterfaceDeclarationSyntax, ProxyData> candidateInterfaces) :
            base(context, candidateInterfaces)
        {
        }

        public IEnumerable<FileData> GenerateFiles()
        {
            foreach (var ci in _candidateInterfaces)
            {
                var file = GenerateFile(ci.Value.InterfaceName, ci.Value.TypeName, ci.Value.ProxyAll);
                files.Add(file);
            }

            return files;
        }

        private FileData GenerateFile(string interfaceName, string typeName, bool proxyAll)
        {
            var symbol = GetType(typeName);

            var file = new FileData(
                $"{interfaceName}.cs",
                CreatePartialInterfaceCode(symbol, interfaceName, proxyAll)
            );

            _context.GeneratedData.Add(new() { InterfaceName = interfaceName, ClassName = null, FileData = file });

            return file;
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

            foreach (var property in MemberHelper.GetPublicProperties(symbol, complexFilters.ToArray()))
            {
                if (proxyAll)
                {
                    var existing = _context.GeneratedData
                        .FirstOrDefault(x => x.ClassName == $"{property.Name}Proxy" || x.InterfaceName == $"I{property.Name}");

                    if (existing is not null)
                    {
                        str.AppendLine($"        {property.ToPropertyText(existing.InterfaceName)}");
                    }
                    else
                    {
                        // Create new
                        var typeName = $"{property.Type}";
                        var file = GenerateFile($"I{property.Name}", typeName, false);
                        str.AppendLine($"        // {property.ToPropertyText($"I{property.Name}")}");
                    }
                }
                else
                {
                    str.AppendLine($"        {property.ToPropertyText()}");
                }
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