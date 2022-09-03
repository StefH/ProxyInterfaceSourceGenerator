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
        foreach (var ci in Context.Candidates)
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

        var interfaceName = ResolveInterfaceNameWithOptionalTypeConstraints(targetClassSymbol.Symbol, pd.ShortInterfaceName);
        var className = targetClassSymbol.Symbol.ResolveProxyClassName();
        var constructorName = $"{targetClassSymbol.Symbol.Name}Proxy";

        var extendsProxyClasses = new List<ProxyData>();
        foreach (var baseType in targetClassSymbol.BaseTypes)
        {
            var candidate = Context.Candidates.Values.FirstOrDefault(ci => ci.FullRawTypeName == baseType.ToString());
            if (candidate is not null)
            {
                extendsProxyClasses.Add(candidate);
                break;
            }

            // Try to find with usings
            foreach (var @using in pd.Usings)
            {
                candidate = Context.Candidates.Values.FirstOrDefault(ci => $"{@using}.{ci.FullRawTypeName}" == baseType.ToString());
                if (candidate is not null)
                {
                    // Update the FullRawTypeName
                    candidate.FullRawTypeName = $"{@using}.{candidate.FullRawTypeName}";

                    extendsProxyClasses.Add(candidate);
                    break;
                }
            }
        }

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
        var firstExtends = extendsProxyClasses.FirstOrDefault();
        var extends = string.Empty; //extendsProxyClasses.Select(e => $"{e.Namespace}.{e.ShortTypeName}Proxy, ").FirstOrDefault() ?? string.Empty;
        var @base = string.Empty; //extendsProxyClasses.Any() ? " : base(instance)" : string.Empty;
        //var @new = extendsProxyClasses.Any() ? "new " : string.Empty;

        var instanceBaseDefinition = string.Empty; //string.Join("\r\n", extendsProxyClasses.Select(x => $"        public {x.FullRawTypeName} _Instance{x.FullRawTypeName.GetLastPart()} {{ get; }}"));
        var instanceBaseSetter = string.Empty; //string.Join("\r\n", extendsProxyClasses.Select(x => $"            _Instance{x.FullRawTypeName.GetLastPart()} = instance;"));

        if (firstExtends is not null)
        {
            extends = $"{firstExtends.Namespace}.{firstExtends.ShortTypeName}Proxy, ";
            @base = " : base(instance)";
            instanceBaseDefinition = $"        public {firstExtends.FullRawTypeName} _Instance{firstExtends.FullRawTypeName.GetLastPart()} {{ get; }}";
            instanceBaseSetter = $"            _Instance{firstExtends.FullRawTypeName.GetLastPart()} = instance;";
        }

        var @abstract = string.Empty; // targetClassSymbol.Symbol.IsAbstract ? "abstract " : string.Empty;
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
    public {@abstract}partial class {className} : {extends}{interfaceName}
    {{
        public {@new}{targetClassSymbol.Symbol} _Instance {{ get; }}
{instanceBaseDefinition}

{properties}

{methods}

{events}

        public {constructorName}({targetClassSymbol} instance){@base}
        {{
            _Instance = instance;
{instanceBaseSetter}

{configurationForAutoMapper}
        }}

{privateAutoMapper}
    }}
}}
{(SupportsNullable ? "#nullable disable" : string.Empty)}";
    }

    private string GeneratePublicProperties(ClassSymbol targetClassSymbol, bool proxyBaseClasses)
    {
        var str = new StringBuilder();

        foreach (var property in MemberHelper.GetPublicProperties(targetClassSymbol, proxyBaseClasses))
        {
            var type = GetPropertyType(property, out var isReplaced);

            var instance = !property.IsStatic ?
                "_Instance" :
                $"{targetClassSymbol.Symbol}";

            var propertyName = property.GetSanitizedName();
            var instancePropertyName = $"{instance}.{propertyName}";
            if (property.IsIndexer)
            {
                var parameters = GetMethodParameters(property.Parameters, true);
                propertyName = $"this[{string.Join(", ", parameters)}]";

                var instanceParameters = GetMethodParameters(property.Parameters, false);
                instancePropertyName = $"{instance}[{string.Join(", ", instanceParameters)}]";
            }

            var overrideOrVirtual = string.Empty;
            if (property.IsOverride)
            {
                overrideOrVirtual = "override ";
            }
            else if (property.IsVirtual)
            {
                overrideOrVirtual = "virtual ";
            }

            string get;
            string set;
            if (isReplaced)
            {
                get = property.GetMethod != null ? $"get => _mapper.Map<{type}>({instancePropertyName}); " : string.Empty;
                set = property.SetMethod != null ? $"set => {instancePropertyName} = _mapper.Map<{property.Type}>(value); " : string.Empty;
            }
            else
            {
                get = property.GetMethod != null ? $"get => {instancePropertyName}; " : string.Empty;
                set = property.SetMethod != null ? $"set => {instancePropertyName} = value; " : string.Empty;
            }

            str.AppendLine($"        public {overrideOrVirtual}{type} {propertyName} {{ {get}{set}}}");
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

            string overrideOrVirtual = string.Empty;
            if (method.IsOverride && method.OverriddenMethod != null)
            {
                var baseType = method.OverriddenMethod.ContainingType.GetFullType();
                if (TryGetNamedTypeSymbolByFullName(TypeKind.Class, baseType, Enumerable.Empty<string>(), out _))
                {
                    overrideOrVirtual = "override ";
                }
            }
            else if (method.IsVirtual)
            {
                overrideOrVirtual = "virtual ";
            }

            string returnTypeAsString = GetReplacedType(method.ReturnType, out var returnIsReplaced);

            var whereStatement = GetWhereStatementFromMethod(method);

            str.AppendLine($"        public {overrideOrVirtual}{returnTypeAsString} {method.GetMethodNameWithOptionalTypeParameters()}({string.Join(", ", methodParameters)}){whereStatement}");
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