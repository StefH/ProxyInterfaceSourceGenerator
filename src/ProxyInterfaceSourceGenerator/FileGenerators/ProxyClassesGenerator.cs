using System;
using System.Collections.Generic;
using System.Linq;
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
        public {symbol} _Instance {{ get; }}

{GeneratePublicProperties(symbol, proxyAll)}

{GeneratePublicMethods(symbol)}

        public {className}Proxy({symbol} instance)
        {{
            _Instance = instance;

{GenerateMapperConfigurationForAutoMapper()}
        }}

{GeneratePrivateAutoMapper()}
    }}
}}";
        private string GeneratePrivateAutoMapper()
        {
            return _context.ReplacedTypes.Count == 0 ? string.Empty : "        private readonly IMapper _mapper;";
        }

        private string GenerateMapperConfigurationForAutoMapper()
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

            foreach (var property in MemberHelper.GetPublicProperties(symbol))
            {
                switch (property.GetTypeEnum())
                {
                    case TypeEnum.ValueTypeOrString:
                    case TypeEnum.Interface:
                        str.AppendLine($"        public {property.ToPropertyTextForClass()}");
                        str.AppendLine();
                        break;

                    default:
                        var type = GetPropertyType(property, out var isReplaced);
                        if (isReplaced)
                        {
                            str.AppendLine($"        public {property.ToPropertyTextForClass(type)}");
                        }
                        else
                        {
                            str.AppendLine($"        public {property.ToPropertyTextForClass()}");
                        }
                        str.AppendLine();
                        break;
                }

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
                        var type = GetParameterType(ps, out var isReplaced);
                        methodParameters.Add($"{ps.GetParamsPrefix()}{ps.GetRefPrefix()}{type} {ps.Name}");
                    }
                    else
                    {
                        methodParameters.Add($"{ps.GetParamsPrefix()}{ps.GetRefPrefix()}{ps.Type} {ps.Name}");
                    }

                    invokeParameters.Add($"{ps.GetRefPrefix()}_{ps.Name}");
                }

                string returnTypeAsString = GetReplacedType(method.ReturnType, out var returnIsReplaced);

                str.AppendLine($"        public {returnTypeAsString} {method.Name}({string.Join(", ", methodParameters)})");
                str.AppendLine("        {");
                foreach (var ps in method.Parameters)
                {
                    string normalOrMap = $" = {ps.Name}";
                    if (ps.RefKind == RefKind.Out)
                    {
                        normalOrMap = string.Empty;
                    }
                    else
                    {
                        var type = GetParameterType(ps, out var isReplaced);
                        if (isReplaced)
                        {
                            normalOrMap = $" = _mapper.Map<{ps.Type}>({ps.Name})";
                        }
                    }

                    str.AppendLine($"             {ps.Type} _{ps.Name}{normalOrMap};");
                }

#pragma warning disable RS1024 // Compare symbols correctly
                int hash = method.ReturnType.GetHashCode();
#pragma warning restore RS1024 // Compare symbols correctly
                var alternateReturnVariableName = $"result_{Math.Abs(hash)}";

                if (returnTypeAsString == "void")
                {
                    str.AppendLine($"             _Instance.{method.Name}({string.Join(", ", invokeParameters)});");
                }
                else
                {
                    str.AppendLine($"             var {alternateReturnVariableName} = _Instance.{method.Name}({string.Join(", ", invokeParameters)});");
                }

                foreach (var ps in method.Parameters.Where(p => p.RefKind == RefKind.Out))
                {
                    string normalOrMap = $" = _{ps.Name}";
                    if (ps.GetTypeEnum() == TypeEnum.Complex)
                    {
                        var type = GetParameterType(ps, out var isReplaced);
                        if (isReplaced)
                        {
                            normalOrMap = $" = _mapper.Map<{type}>(_{ps.Name})";
                        }
                    }

                    str.AppendLine($"             {ps.Name}{normalOrMap};");
                }

                if (returnTypeAsString != "void")
                {
                    if (returnIsReplaced)
                    {
                        str.AppendLine($"             return _mapper.Map<{returnTypeAsString}>({alternateReturnVariableName});");
                    }
                    else
                    {
                        str.AppendLine($"             return {alternateReturnVariableName};");
                    }
                }

                str.AppendLine("        }");
                str.AppendLine();
            }

            return str.ToString();
        }

        private string GeneratePublicMethodsOld(INamedTypeSymbol symbol)
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
                        var type = GetParameterType(ps, out var isReplaced);
                        methodParameters.Add($"{ps.GetParamsPrefix()}{ps.GetRefPrefix()}{type} {ps.Name}");

                        if (isReplaced)
                        {
                            invokeParameters.Add($"_mapper.Map<{ps.Type}>({ps.Name})");
                        }
                        else
                        {
                            invokeParameters.Add($"{ps.GetRefPrefix()}{ps.Name}");
                        }
                    }
                    else
                    {
                        methodParameters.Add($"{ps.GetParamsPrefix()}{ps.GetRefPrefix()}{ps.Type} {ps.Name}");

                        invokeParameters.Add($"{ps.GetRefPrefix()}{ps.Name}");
                    }
                }

                string returnTypeAsString;
                string call = $"_Instance.{method.Name}({string.Join(", ", invokeParameters)})";
                if (method.ReturnType.GetTypeEnum() == TypeEnum.Complex)
                {
                    returnTypeAsString = GetReplacedType(method.ReturnType, out var isReplaced);
                    if (isReplaced)
                    {
                        call = $"_mapper.Map<{returnTypeAsString}>(_Instance.{method.Name}({string.Join(", ", invokeParameters)}))";
                    }
                }
                else
                {
                    returnTypeAsString = method.ReturnType.ToString();
                }

                str.AppendLine($"        public {returnTypeAsString} {method.Name}({string.Join(", ", methodParameters)}) => {call};");
                str.AppendLine();
            }

            return str.ToString();
        }
    }
}