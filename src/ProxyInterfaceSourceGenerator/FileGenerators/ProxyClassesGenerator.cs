using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Enums;
using ProxyInterfaceSourceGenerator.Extensions;
using ProxyInterfaceSourceGenerator.Utils;

namespace ProxyInterfaceSourceGenerator.FileGenerators
{
    internal class ProxyClassesGenerator : BaseGenerator, IFilesGenerator
    {
        public ProxyClassesGenerator(Context context) : base(context)
        {
        }

        public IEnumerable<FileData> GenerateFiles()
        {
            foreach (var ci in _context.CandidateInterfaces)
            {
                yield return GenerateFile(ci.Value.Namespace, ci.Value.InterfaceName, ci.Value.ClassName, ci.Value.TypeName, ci.Value.ProxyAll);
            }
        }

        private FileData GenerateFile(string ns, string interfaceName, string className, string typeName, bool proxyAll)
        {
            var symbol = GetType(typeName);

            var file = new FileData(
                $"{className}Proxy.cs",
                CreateProxyClassCode(ns, symbol, interfaceName, className, proxyAll)
            );

            _context.GeneratedData.Add(new() { InterfaceName = interfaceName, ClassName = className, FileData = file });

            return file;
        }

        private string CreateProxyClassCode(string ns, INamedTypeSymbol symbol, string interfaceName, string className, bool proxyAll) => $@"using System;
using AutoMapper;

namespace {ns}
{{
    public class {className}Proxy : {interfaceName}
    {{
        private readonly IMapper _mapper;

        public {symbol} _Instance {{ get; }}

{GeneratePublicProperties(symbol, proxyAll)}

{GeneratePublicMethods(symbol)}

        public {className}Proxy({symbol} instance)
        {{
            _Instance = instance;

{GenerateAutoMapper()}
        }}
    }}
}}";

        private string GenerateAutoMapper()
        {
            if (_context.ReplacedTypes.Count == 0)
            {
                return string.Empty;
            }

            var str = new StringBuilder();

            str.AppendLine("            _mapper = new MapperConfiguration(cfg =>");
            str.AppendLine("            {");
            foreach (var replacedType in _context.ReplacedTypes)
            {
                str.AppendLine($"                cfg.CreateMap<{replacedType.Key}, {replacedType.Value}>();");
                str.AppendLine($"                cfg.CreateMap<{replacedType.Value}, {replacedType.Key}>();");
            }
            str.AppendLine("            }).CreateMapper();");

            return str.ToString();
        }

        private string GeneratePublicProperties(INamedTypeSymbol symbol, bool proxyAll)
        {
            var str = new StringBuilder();

            // SimpleProperties
            foreach (var property in MemberHelper.GetPublicProperties(symbol, p => p.GetTypeEnum() == TypeEnum.ValueTypeOrString))
            {
                str.AppendLine($"        public {property.ToPropertyTextForClass()}");
                str.AppendLine();
            }

            // InterfaceProperties
            foreach (var property in MemberHelper.GetPublicProperties(symbol, p => p.GetTypeEnum() == TypeEnum.Interface))
            {
                str.AppendLine($"        public {property.ToPropertyTextForClass()}");
                str.AppendLine();
            }

            // ComplexProperties
            foreach (var property in MemberHelper.GetPublicProperties(symbol, p => p.GetTypeEnum() == TypeEnum.Complex))
            {
                var type = GetPropertyType(property);
                str.AppendLine($"        public {property.ToPropertyTextForClass(type)}");
                str.AppendLine();
            }

            return str.ToString();
        }

        private string GeneratePublicMethods(INamedTypeSymbol symbol)
        {
            var str = new StringBuilder();
            foreach (var method in MemberHelper.GetPublicMethods(symbol))
            {
                var methodParameters = new List<string>();
                var invokeParameters = new List<string>();

                foreach (var ps in method.Parameters)
                {
                    if (ps.GetTypeEnum() == TypeEnum.Complex)
                    {
                        methodParameters.Add($"{GetParameterType(ps)} {ps.Name}");

                        invokeParameters.Add($"_mapper.Map<{ps.Type}>({ps.Name})");
                    }
                    else
                    {
                        methodParameters.Add($"{ps.Type} {ps.Name}");

                        invokeParameters.Add($"{ps.Name}");
                    }
                }

                string returnTypeAsString;
                string call;
                if (method.ReturnType.GetTypeEnum() == TypeEnum.Complex)
                {
                    returnTypeAsString = GetReplacedType(method.ReturnType);
                    call = $"_mapper.Map<{returnTypeAsString}>(_Instance.{method.Name}({string.Join(", ", invokeParameters)}))";
                }
                else
                {
                    returnTypeAsString = method.ReturnType.ToString();
                    call = $"_Instance.{method.Name}({string.Join(", ", invokeParameters)})";
                }

                str.AppendLine($"        public {returnTypeAsString} {method.Name}({string.Join(", ", methodParameters)}) => {call};");
                str.AppendLine();
            }

            return str.ToString();
        }
    }
}