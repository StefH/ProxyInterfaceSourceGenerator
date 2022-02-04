using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;
using ProxyInterfaceSourceGenerator.Enums;
using ProxyInterfaceSourceGenerator.Extensions;
using ProxyInterfaceSourceGenerator.Models;
using ProxyInterfaceSourceGenerator.Utils;

namespace ProxyInterfaceSourceGenerator.FileGenerators;

internal partial class ProxyClassesGenerator : BaseGenerator, IFilesGenerator
{
    public ProxyClassesGenerator(Context context, bool supportsNullable) : base(context, supportsNullable)
    {
    }

    public IEnumerable<FileData> GenerateFiles()
    {
        foreach (var ci in Context.CandidateInterfaces)
        {
            if (TryGenerateFile(ci.Value, out var file))
            {
                yield return file;
            }
        }
    }

    [SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1024:Compare symbols correctly", Justification = "<Pending>")]
    private bool TryGenerateFile(ProxyData pd, [NotNullWhen(true)] out FileData? fileData)
    {
        fileData = default;

        if (!TryGetNamedTypeSymbolByFullName(TypeKind.Class, pd.FullTypeName, pd.Usings, out var targetClassSymbol))
        {
            return false;
        }

        var interfaceName = targetClassSymbol.Symbol.ResolveInterfaceNameWithOptionalTypeConstraints(pd.ShortInterfaceName);
        var className = targetClassSymbol.Symbol.ResolveProxyClassName();
        var constructorName = $"{targetClassSymbol.Symbol.Name}Proxy";

        var extendsProxyClasses = targetClassSymbol.BaseTypes
            .Join(
                Context.CandidateInterfaces.Values, // .Select(v => v.RawTypeName)
                namedTypeSymbol => namedTypeSymbol.ToString(),
                proxyData => proxyData.FullRawTypeName, (_, proxyData) => proxyData
            ).ToList();

        fileData = new FileData(
            $"{targetClassSymbol.Symbol.GetFileName()}Proxy.g.cs",
            CreateProxyClassCode(pd, targetClassSymbol, extendsProxyClasses, interfaceName, className, constructorName)
        );

        return true;
    }

    private string CreateProxyClassCode(
        ProxyData pd,
        ClassSymbol targetClassSymbol,
        List<ProxyData> extendsProxyClasses,
        string interfaceName,
        string className,
        string constructorName)
    {
        var extends = extendsProxyClasses.Select(e => $"{e.Namespace}.{e.ShortTypeName}Proxy, ").FirstOrDefault() ?? string.Empty;
        // var extends = extendsFullName != null ? $"{extendsFullName}, " : string.Empty;
        var @base = extendsProxyClasses.Any() ? " : base(instance)" : string.Empty;
        var @new = extendsProxyClasses.Any() ? "new " : string.Empty;
        var instanceBaseDefinition = extendsProxyClasses.Any() ? $"public {extendsProxyClasses[0].FullRawTypeName} _InstanceBase {{ get; }}\r\n" : string.Empty;
        var instanceBaseSet = extendsProxyClasses.Any() ? "_InstanceBase = instance;" : string.Empty;

        var properties = GeneratePublicProperties(targetClassSymbol, pd.ProxyBaseClasses);
        var methods = GeneratePublicMethods(targetClassSymbol, pd.ProxyBaseClasses, extendsProxyClasses);
        var events = GenerateEvents(targetClassSymbol, pd.ProxyBaseClasses);

        var configurationForAutoMapper = string.Empty;
        var privateAutoMapper = string.Empty;
        var usingAutoMapper = string.Empty;
        if (Context.ReplacedTypes.Any())
        {
            configurationForAutoMapper = GenerateMapperConfigurationForAutoMapper();
            privateAutoMapper = GeneratePrivateAutoMapper();
            usingAutoMapper = "using AutoMapper;";
        }

        return $@"//----------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by https://github.com/StefH/ProxyInterfaceSourceGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//----------------------------------------------------------------------------------------

{(SupportsNullable ? "#nullable enable" : string.Empty)}
using System;
{usingAutoMapper}

namespace {pd.Namespace}
{{
    public partial class {className} : {extends}{interfaceName}
    {{
        public {@new}{targetClassSymbol.Symbol} _Instance {{ get; }}
        {instanceBaseDefinition}

{properties}

{methods}

{events}

        public {constructorName}({targetClassSymbol} instance){@base}
        {{
            _Instance = instance;
            {instanceBaseSet}

{configurationForAutoMapper}
        }}

{privateAutoMapper}
    }}
}}
{(SupportsNullable ? "#nullable disable" : string.Empty)}";
    }

    private static string GeneratePrivateAutoMapper()
    {
        return "        private readonly IMapper _mapper;";
    }

    private string GenerateMapperConfigurationForAutoMapper()
    {
        var str = new StringBuilder();

        str.AppendLine("            _mapper = new MapperConfiguration(cfg =>");
        str.AppendLine("            {");
        foreach (var replacedType in Context.ReplacedTypes)
        {
            var fullTypeName = Context.CandidateInterfaces.First(ci => ci.Value.FullTypeName == replacedType.Key);
            var classNameProxy = $"{fullTypeName.Value.Namespace}.{fullTypeName.Value.ShortTypeName}Proxy";

            var instance = $"instance{(replacedType.Key + replacedType.Value).GetDeterministicHashCodeAsString()}";
            var proxy = $"proxy{(replacedType.Value + replacedType.Key).GetDeterministicHashCodeAsString()}";

            str.AppendLine($"                cfg.CreateMap<{replacedType.Key}, {replacedType.Value}>().ConstructUsing({instance} => new {classNameProxy}({instance}));");
            str.AppendLine($"                cfg.CreateMap<{replacedType.Value}, {replacedType.Key}>().ConstructUsing({proxy} => (({classNameProxy}) {proxy})._Instance);");
        }
        str.AppendLine("            }).CreateMapper();");

        return str.ToString();
    }

    private string GeneratePublicProperties(ClassSymbol targetClassSymbol, bool proxyBaseClasses)
    {
        var str = new StringBuilder();

        foreach (var property in MemberHelper.GetPublicProperties(targetClassSymbol, proxyBaseClasses))
        {
            var type = GetPropertyType(property, out var isReplaced);
            if (isReplaced)
            {
                str.AppendLine($"        public {property.ToPropertyTextForClass(targetClassSymbol, type)}");
            }
            else
            {
                str.AppendLine($"        public {property.ToPropertyTextForClass(targetClassSymbol)}");
            }
            str.AppendLine();
        }

        return str.ToString();
    }

    private string GeneratePublicMethods(ClassSymbol targetClassSymbol, bool proxyBaseClasses, List<ProxyData> extendsProxyClasses)
    {
        var str = new StringBuilder();
        foreach (var method in MemberHelper.GetPublicMethods(targetClassSymbol, proxyBaseClasses))
        {
            var methodParameters = new List<string>();
            var invokeParameters = new List<string>();

            foreach (var ps in method.Parameters)
            {
                var type = GetParameterType(ps, out _);

                methodParameters.Add($"{ps.GetParamsPrefix()}{ps.GetRefPrefix()}{type} {ps.GetSanitizedName()}{ps.GetDefaultValue()}");
                invokeParameters.Add($"{ps.GetRefPrefix()}{ps.GetSanitizedName()}_");
            }

            string addNew = string.Empty;
            if (method.IsOverride && method.OverriddenMethod != null)
            {
                var baseType = method.OverriddenMethod.ContainingType.GetFullType();
                if (TryGetNamedTypeSymbolByFullName(TypeKind.Class, baseType, Enumerable.Empty<string>(), out var baseTypeClassSymbol))
                {
                    addNew = "new ";
                }
            }

            string returnTypeAsString = GetReplacedType(method.ReturnType, out var returnIsReplaced);

            str.AppendLine($"        public {addNew}{returnTypeAsString} {method.GetMethodNameWithOptionalTypeParameters()}({string.Join(", ", methodParameters)}){method.GetWhereStatement()}");
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

                str.AppendLine($"            {ps.Type} {ps.GetSanitizedName()}_{normalOrMap};");
            }

            var methodName = method.GetMethodNameWithOptionalTypeParameters();
            var alternateReturnVariableName = $"result_{methodName.GetDeterministicHashCodeAsString()}";

            string instance = !method.IsStatic ?
                "_Instance" :
                $"{targetClassSymbol.Symbol}";

            if (returnTypeAsString == "void")
            {
                str.AppendLine($"            {instance}.{methodName}({string.Join(", ", invokeParameters)});");
            }
            else
            {
                str.AppendLine($"            var {alternateReturnVariableName} = {instance}.{methodName}({string.Join(", ", invokeParameters)});");
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

                str.AppendLine($"            {ps.GetSanitizedName()}{normalOrMap};");
            }

            if (returnTypeAsString != "void")
            {
                if (returnIsReplaced)
                {
                    str.AppendLine($"            return _mapper.Map<{returnTypeAsString}>({alternateReturnVariableName});");
                }
                else
                {
                    str.AppendLine($"            return {alternateReturnVariableName};");
                }
            }

            str.AppendLine("        }");
            str.AppendLine();
        }

        return str.ToString();
    }

    private string GenerateEvents(ClassSymbol targetClassSymbol, bool proxyBaseClasses)
    {
        var str = new StringBuilder();
        foreach (var @event in MemberHelper.GetPublicEvents(targetClassSymbol, proxyBaseClasses))
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