﻿using System;
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
                        var type = GetParameterType(ps, out _);
                        methodParameters.Add($"{ps.GetParamsPrefix()}{ps.GetRefPrefix()}{type} {ps.SanitizedName()}");
                    }
                    else
                    {
                        methodParameters.Add($"{ps.GetParamsPrefix()}{ps.GetRefPrefix()}{ps.Type} {ps.SanitizedName()}");
                    }

                    invokeParameters.Add($"{ps.GetRefPrefix()}{ps.SanitizedName()}_");
                }

                string returnTypeAsString = GetReplacedType(method.ReturnType, out var returnIsReplaced);

                str.AppendLine($"        public {returnTypeAsString} {method.Name}({string.Join(", ", methodParameters)})");
                str.AppendLine("        {");
                foreach (var ps in method.Parameters)
                {
                    string normalOrMap = $" = {ps.SanitizedName()}";
                    if (ps.RefKind == RefKind.Out)
                    {
                        normalOrMap = string.Empty;
                    }
                    else
                    {
                        var type = GetParameterType(ps, out var isReplaced);
                        if (isReplaced)
                        {
                            normalOrMap = $" = _mapper.Map<{ps.Type}>({ps.SanitizedName()})";
                        }
                    }

                    str.AppendLine($"             {ps.Type} {ps.SanitizedName()}_{normalOrMap};");
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
                    string normalOrMap = $" = {ps.SanitizedName()}_";
                    if (ps.GetTypeEnum() == TypeEnum.Complex)
                    {
                        var type = GetParameterType(ps, out var isReplaced);
                        if (isReplaced)
                        {
                            normalOrMap = $" = _mapper.Map<{type}>({ps.SanitizedName()}_)";
                        }
                    }

                    str.AppendLine($"             {ps.SanitizedName()}{normalOrMap};");
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
    }
}