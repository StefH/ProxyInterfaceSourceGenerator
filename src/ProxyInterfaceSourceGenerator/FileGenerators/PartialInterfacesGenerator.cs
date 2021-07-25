using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Enums;
using ProxyInterfaceSourceGenerator.Extensions;
using ProxyInterfaceSourceGenerator.Utils;

namespace ProxyInterfaceSourceGenerator.FileGenerators
{
    internal class PartialInterfacesGenerator : BaseGenerator, IFilesGenerator
    {
        public PartialInterfacesGenerator(Context context) :
            base(context)
        {
        }

        public IEnumerable<FileData> GenerateFiles()
        {
            foreach (var ci in _context.CandidateInterfaces)
            {
                yield return GenerateFile(ci.Value.Namespace, ci.Value.InterfaceName, ci.Value.TypeName, ci.Value.ProxyAll);
            }
        }

        private FileData GenerateFile(string ns, string interfaceName, string typeName, bool proxyAll)
        {
            var symbol = GetType(typeName);

            var file = new FileData(
                $"{interfaceName}.cs",
                CreatePartialInterfaceCode(ns, symbol, interfaceName, proxyAll)
            );

            _context.GeneratedData.Add(new() { InterfaceName = interfaceName, ClassName = null, FileData = file });

            return file;
        }

        private string CreatePartialInterfaceCode(string ns, INamedTypeSymbol symbol, string interfaceName, bool proxyAll) => $@"using System;

namespace {ns}
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
                var type = GetPropertyType(property);
                str.AppendLine($"        {property.ToPropertyText(type)}");
                str.AppendLine();
            }

            return str.ToString();
        }

        private string GenerateMethods(INamedTypeSymbol symbol)
        {
            var str = new StringBuilder();
            foreach (var method in MemberHelper.GetPublicMethods(symbol))
            {
                var methodParameters = new List<string>();
                foreach (var ps in method.Parameters)
                {
                    if (ps.GetTypeEnum() == TypeEnum.Complex)
                    {
                        var type = GetParameterType(ps);
                        methodParameters.Add($"{type} {ps.Name}");
                    }
                    else
                    {
                        methodParameters.Add($"{ps.Type} {ps.Name}");
                    }
                }

                str.AppendLine($"        {GetReplacedType(method.ReturnType)} {method.Name}({string.Join(", ", methodParameters)});");
                str.AppendLine();
            }

            return str.ToString();
        }
    }
}