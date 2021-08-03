using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Enums;
using ProxyInterfaceSourceGenerator.Extensions;
using ProxyInterfaceSourceGenerator.SyntaxReceiver;
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
                yield return GenerateFile(ci.Value);
            }
        }

        private FileData GenerateFile(ProxyData pd)
        {
            var targetClassSymbol = GetNamedTypeSymbolByFullName(pd.TypeName, pd.Usings);
            var interfaceName = targetClassSymbol.ResolveInterfaceNameWithOptionalTypeConstraints(pd.InterfaceName);
            var className = targetClassSymbol.ResolveProxyClassName();
            var constructorName = $"{targetClassSymbol.Name}Proxy";

            var file = new FileData(
                $"{pd.FileName}Proxy.cs",
                CreateProxyClassCode(pd.Namespace, targetClassSymbol, interfaceName, className, constructorName)
            );

            // _context.GeneratedData.Add(new() { InterfaceName = interfaceName, ClassName = pd.ClassName, FileData = file });

            return file;
        }

        private string CreateProxyClassCode(string ns, INamedTypeSymbol targetClassSymbol, string interfaceName, string className, string constructorName) => $@"using System;
using AutoMapper;

namespace {ns}
{{
    public class {className} : {interfaceName}
    {{
        public {targetClassSymbol} _Instance {{ get; }}

{GeneratePublicProperties(targetClassSymbol, false)}

{GeneratePublicMethods(targetClassSymbol)}

{GenerateEvents(targetClassSymbol)}

        public {constructorName}({targetClassSymbol} instance)
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

        private string GeneratePublicProperties(INamedTypeSymbol targetClassSymbol, bool proxyAll)
        {
            var str = new StringBuilder();

            foreach (var property in MemberHelper.GetPublicProperties(targetClassSymbol))
            {
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
            }

            return str.ToString();
        }

        private string GeneratePublicMethods(INamedTypeSymbol targetClassSymbol)
        {
            var str = new StringBuilder();
            foreach (var method in MemberHelper.GetPublicMethods(targetClassSymbol))
            {
                var methodParameters = new List<string>();
                var invokeParameters = new List<string>();

                foreach (var ps in method.Parameters)
                {
                    var type = GetParameterType(ps, out _);

                    methodParameters.Add($"{ps.GetParamsPrefix()}{ps.GetRefPrefix()}{type} {ps.GetSanitizedName()}{ps.GetDefaultValue()}");
                    invokeParameters.Add($"{ps.GetRefPrefix()}{ps.GetSanitizedName()}_");
                }

                string returnTypeAsString = GetReplacedType(method.ReturnType, out var returnIsReplaced);

                str.AppendLine($"        public {returnTypeAsString} {method.GetMethodNameWithOptionalTypeParameters()}({string.Join(", ", methodParameters)}){method.GetWhereStatement()}");
                str.AppendLine("        {");
                foreach (var ps in method.Parameters)
                {
                    string normalOrMap = $" = {ps.GetSanitizedName()}";
                    if (ps.RefKind == RefKind.Out)
                    {
                        normalOrMap = string.Empty;
                    }
                    else
                    {
                        var type = GetParameterType(ps, out var isReplaced);
                        if (isReplaced)
                        {
                            normalOrMap = $" = _mapper.Map<{ps.Type}>({ps.GetSanitizedName()})";
                        }
                    }

                    str.AppendLine($"             {ps.Type} {ps.GetSanitizedName()}_{normalOrMap};");
                }

#pragma warning disable RS1024 // Compare symbols correctly
                int hash = method.ReturnType.GetHashCode();
#pragma warning restore RS1024 // Compare symbols correctly
                var alternateReturnVariableName = $"result_{Math.Abs(hash)}";

                if (returnTypeAsString == "void")
                {
                    str.AppendLine($"             _Instance.{method.GetMethodNameWithOptionalTypeParameters()}({string.Join(", ", invokeParameters)});");
                }
                else
                {
                    str.AppendLine($"             var {alternateReturnVariableName} = _Instance.{method.GetMethodNameWithOptionalTypeParameters()}({string.Join(", ", invokeParameters)});");
                }

                foreach (var ps in method.Parameters.Where(p => p.RefKind == RefKind.Out))
                {
                    string normalOrMap = $" = {ps.GetSanitizedName()}_";
                    if (ps.GetTypeEnum() == TypeEnum.Complex)
                    {
                        var type = GetParameterType(ps, out var isReplaced);
                        if (isReplaced)
                        {
                            normalOrMap = $" = _mapper.Map<{type}>({ps.GetSanitizedName()}_)";
                        }
                    }

                    str.AppendLine($"             {ps.GetSanitizedName()}{normalOrMap};");
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

        private string GenerateEvents(INamedTypeSymbol targetClassSymbol)
        {
            var str = new StringBuilder();
            foreach (var @event in MemberHelper.GetPublicEvents(targetClassSymbol))
            {
                var name = @event.Key.GetSanitizedName();
                var ps = @event.First().Parameters.First();
                var type = ps.GetTypeEnum() == TypeEnum.Complex ? GetParameterType(ps, out _) : ps.Type.ToString();
                str.Append($"        public event {type} {name} {{");

                if (@event.Any(e => e.MethodKind == MethodKind.EventAdd))
                {
                    str.Append($" add {{ _Instance.{name} += value; }}");
                }
                if (@event.Any(e => e.MethodKind == MethodKind.EventRemove))
                {
                    str.Append($" remove {{ _Instance.{name} -= value; }}");
                }

                str.AppendLine(" }");
                str.AppendLine();
            }

            return str.ToString();
        }
    }
}