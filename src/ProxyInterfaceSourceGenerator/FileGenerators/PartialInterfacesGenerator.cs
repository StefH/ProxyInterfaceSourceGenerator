using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Enums;
using ProxyInterfaceSourceGenerator.Extensions;
using ProxyInterfaceSourceGenerator.Utils;

namespace ProxyInterfaceSourceGenerator.FileGenerators
{
    internal class PartialInterfacesGenerator : BaseGenerator, IFilesGenerator
    {
        private readonly List<FileData> files = new List<FileData>();

        public PartialInterfacesGenerator(Context context) :
            base(context)
        {
        }

        public IEnumerable<FileData> GenerateFiles()
        {
            foreach (var ci in _context.CandidateInterfaces)
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
            foreach (var property in MemberHelper.GetPublicProperties(symbol, p => p.GetTypeEnum() == TypeEnum.ValueTypeOrString))
            {
                str.AppendLine($"        {property.ToPropertyText()}");
                str.AppendLine();
            }

            // InterfaceProperties
            foreach (var property in MemberHelper.GetPublicProperties(symbol, p => p.GetTypeEnum() == TypeEnum.Interface))
            {
                str.AppendLine($"        {property.ToPropertyText()}");
                str.AppendLine();
            }

            // ComplexProperties
            foreach (var property in MemberHelper.GetPublicProperties(symbol, p => p.GetTypeEnum() == TypeEnum.Complex))
            {
                //if (proxyAll)
                //{
                //    var existing = _context.GeneratedData
                //        .FirstOrDefault(x => x.ClassName == $"{property.Name}Proxy" || x.InterfaceName == $"I{property.Name}");

                //    if (existing is not null)
                //    {
                //        str.AppendLine($"        {property.ToPropertyText(existing.InterfaceName)}");
                //    }
                //    else
                //    {
                //        // Create new
                //        var typeName = $"{property.Type}";
                //        var file = GenerateFile($"I{property.Name}", typeName, false);
                //        str.AppendLine($"        // {property.ToPropertyText($"I{property.Name}")}");
                //    }
                //}
                //else

                var existing = _context.CandidateInterfaces.Values.FirstOrDefault(x => x.TypeName == property.Type.ToString());
                if (existing is not null)
                {
                    str.AppendLine($"        {property.ToPropertyText(existing.InterfaceName)}");
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
                str.AppendLine($"        {method.ToMethodText()};");
                str.AppendLine();
            }

            return str.ToString();
        }
    }
}