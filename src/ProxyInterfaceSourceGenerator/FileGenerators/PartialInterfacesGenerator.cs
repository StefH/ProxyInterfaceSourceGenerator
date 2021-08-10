using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProxyInterfaceSourceGenerator.Enums;
using ProxyInterfaceSourceGenerator.Extensions;
using ProxyInterfaceSourceGenerator.SyntaxReceiver;
using ProxyInterfaceSourceGenerator.Utils;

namespace ProxyInterfaceSourceGenerator.FileGenerators
{
    internal class PartialInterfacesGenerator : BaseGenerator, IFilesGenerator
    {
        public PartialInterfacesGenerator(Context context, bool supportsNullable) :
            base(context, supportsNullable)
        {
        }

        public IEnumerable<FileData> GenerateFiles()
        {
            foreach (var ci in _context.CandidateInterfaces)
            {
                yield return GenerateFile(ci.Key, ci.Value);
            }
        }

        private FileData GenerateFile(InterfaceDeclarationSyntax ci, ProxyData pd)
        {
            var sourceInterfaceSymbol = GetNamedTypeSymbolByFullName(ci.Identifier.ToString(), pd.Usings);
            var targetClassSymbol = GetNamedTypeSymbolByFullName(pd.TypeName, pd.Usings);
            var interfaceName = targetClassSymbol.ResolveInterfaceNameWithOptionalTypeConstraints(pd.InterfaceName);

            var file = new FileData(
                $"{sourceInterfaceSymbol.GetFileName()}.g.cs",
                CreatePartialInterfaceCode(pd.Namespace, targetClassSymbol, interfaceName, pd.ProxyAll)
            );

            // _context.GeneratedData.Add(new() { InterfaceName = interfaceName, ClassName = null, FileData = file });

            return file;
        }

        private string CreatePartialInterfaceCode(string ns, INamedTypeSymbol targetClassSymbol, string interfaceName, bool proxyAll) => $@"//----------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by https://github.com/StefH/ProxyInterfaceSourceGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//----------------------------------------------------------------------------------------

{(_supportsNullable ? "#nullable enable" : string.Empty)}
using System;

namespace {ns}
{{
    public partial interface {interfaceName}
    {{
{GenerateProperties(targetClassSymbol, proxyAll)}

{GenerateMethods(targetClassSymbol)}

{GenerateEvents(targetClassSymbol)}
    }}
}}
{(_supportsNullable ? "#nullable disable" : string.Empty)}";

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
                var ps = @event.First().Parameters.First();
                var type = ps.GetTypeEnum() == TypeEnum.Complex ? GetParameterType(ps, out _) : ps.Type.ToString();
                str.AppendLine($"        event {type} {@event.Key.GetSanitizedName()};");
                str.AppendLine();
            }

            return str.ToString();
        }
    }
}