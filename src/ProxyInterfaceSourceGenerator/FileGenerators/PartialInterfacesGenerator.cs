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

            foreach (var property in MemberHelper.GetPublicProperties(symbol))
            {
                switch (property.GetTypeEnum())
                {
                    case TypeEnum.ValueTypeOrString:
                    case TypeEnum.Interface:
                        str.AppendLine($"        {property.ToPropertyText()}");
                        str.AppendLine();
                        break;

                    default:
                        var type = GetPropertyType(property, out var isReplaced);
                        if (isReplaced)
                        {
                            str.AppendLine($"        {property.ToPropertyText(type)}");
                        }
                        else
                        {
                            str.AppendLine($"        {property.ToPropertyText()}");
                        }
                        str.AppendLine();
                        break;
                }
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
                    var type = ps.GetTypeEnum() == TypeEnum.Complex ? GetParameterType(ps, out _) : ps.Type.ToString();
                    methodParameters.Add($"{ps.GetParamsPrefix()}{ps.GetRefPrefix()}{type} {ps.GetSanitizedName()}{ps.GetDefaultValue()}");
                }

                str.AppendLine($"        {GetReplacedType(method.ReturnType, out _)} {method.Name}({string.Join(", ", methodParameters)});");
                str.AppendLine();
            }

            return str.ToString();
        }
    }
}