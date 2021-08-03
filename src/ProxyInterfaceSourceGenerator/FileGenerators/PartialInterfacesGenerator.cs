using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Enums;
using ProxyInterfaceSourceGenerator.Extensions;
using ProxyInterfaceSourceGenerator.SyntaxReceiver;
using ProxyInterfaceSourceGenerator.Utils;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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
                yield return GenerateFile(ci.Value);
            }
        }

        private FileData GenerateFile(ProxyData pd)
        {
            var targetClassSymbol = GetNamedTypeSymbolByFullName(pd.TypeName, pd.Usings);
            var interfaceName = targetClassSymbol.ResolveInterfaceNameWithOptionalTypeConstraints(pd.InterfaceName);

            var file = new FileData(
                $"{pd.FileName}.cs",
                CreatePartialInterfaceCode(pd.Namespace, targetClassSymbol, interfaceName, pd.ProxyAll)
            );

            // _context.GeneratedData.Add(new() { InterfaceName = interfaceName, ClassName = null, FileData = file });

            return file;
        }

        private string CreatePartialInterfaceCode(string ns, INamedTypeSymbol targetClassSymbol, string interfaceName, bool proxyAll) => $@"using System;

namespace {ns}
{{
    public partial interface {interfaceName}
    {{
{GenerateProperties(targetClassSymbol, proxyAll)}

{GenerateMethods(targetClassSymbol)}

{GenerateEvents(targetClassSymbol)}
    }}
}}";

        private string GenerateProperties(INamedTypeSymbol targetClassSymbol, bool proxyAll)
        {
            var str = new StringBuilder();

            foreach (var property in MemberHelper.GetPublicProperties(targetClassSymbol))
            {
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
            }

            return str.ToString();
        }

        private string GenerateMethods(INamedTypeSymbol targetClassSymbol)
        {
            var str = new StringBuilder();
            foreach (var method in MemberHelper.GetPublicMethods(targetClassSymbol))
            {
                var methodParameters = new List<string>();
                foreach (var ps in method.Parameters)
                {
                    var type = ps.GetTypeEnum() == TypeEnum.Complex ? GetParameterType(ps, out _) : ps.Type.ToString();
                    methodParameters.Add($"{ps.GetParamsPrefix()}{ps.GetRefPrefix()}{type} {ps.GetSanitizedName()}{ps.GetDefaultValue()}");
                }

                str.AppendLine($"        {GetReplacedType(method.ReturnType, out _)} {method.GetMethodNameWithOptionalTypeParameters()}({string.Join(", ", methodParameters)}){method.GetWhereStatement()};");
                str.AppendLine();
            }

            return str.ToString();
        }

        private string GenerateEvents(INamedTypeSymbol targetClassSymbol)
        {
            var str = new StringBuilder();
            foreach (var @event in MemberHelper.GetPublicEvents(targetClassSymbol))
            {
                var methodParameters = new List<string>();
                //foreach (var ps in @event.Parameters)
                //{
                //    var type = ps.GetTypeEnum() == TypeEnum.Complex ? GetParameterType(ps, out _) : ps.Type.ToString();
                //    methodParameters.Add($"{ps.GetParamsPrefix()}{ps.GetRefPrefix()}{type} {ps.GetSanitizedName()}{ps.GetDefaultValue()}");
                //}

                //str.AppendLine($"        {GetReplacedType(method.ReturnType, out _)} {method.GetMethodNameWithOptionalTypeParameters()}({string.Join(", ", methodParameters)}){method.GetWhereStatement()};");
                //str.AppendLine();
            }

            return str.ToString();
        }
    }
}