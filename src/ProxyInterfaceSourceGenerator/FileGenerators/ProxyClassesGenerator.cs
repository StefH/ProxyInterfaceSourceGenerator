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

        var interfaceName = ResolveInterfaceNameWithOptionalTypeConstraints(targetClassSymbol.Symbol, pd.ShortInterfaceName);
        var className = targetClassSymbol.Symbol.ResolveProxyClassName();
        var constructorName = $"{targetClassSymbol.Symbol.Name}Proxy";

        var extendsProxyClasses = targetClassSymbol.BaseTypes
            .Join(
                Context.CandidateInterfaces.Values,
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

    private string GeneratePublicProperties(ClassSymbol targetClassSymbol, bool proxyBaseClasses)
    {
        var str = new StringBuilder();

        foreach (var property in MemberHelper.GetPublicProperties(targetClassSymbol, proxyBaseClasses))
        {
            var type = GetPropertyType(property, out var isReplaced);

            var instance = !property.IsStatic ?
                "_Instance" :
                $"{targetClassSymbol.Symbol}";

            string propertyName = property.GetSanitizedName();
            if (isReplaced)
            {

            }
            else
            {
                if (property.IsIndexer)
                {
                    var parameters = GetMethodParameters(property.Parameters.ToArray());
                    propertyName = $"this[{string.Join(",", parameters)}]";
                }

                str.AppendLine($"        public {property.Type} {propertyName} {getSet}");
            }

            /*
             * //var get = property.GetMethod != null ? $"get => {instance}.{property.GetSanitizedName()}; " : string.Empty;
        //var set = property.SetMethod != null ? $"set => {instance}.{property.GetSanitizedName()} = value; " : string.Empty;

        //return $"{property.Type} {property.GetSanitizedName()} {{ {get}{set}}}";
        return (property.Type.ToString(), property.GetSanitizedName(), instance, property.GetMethod != null, property.SetMethod != null);
             */

            //(string propertyType, string? propertyName, string getSet) = isReplaced ?
            //    property.ToPropertyDetailsForClass(targetClassSymbol, type) :
            //    property.ToPropertyDetailsForClass(targetClassSymbol);

            

            // public ProxyInterfaceSourceGeneratorTests.Source.MyStruct this[int i] { get => _Instance[i]; set => _Instance[i] = value; }


            /*
            if (isReplaced)
            {
                str.AppendLine($"        public {property.ToPropertyDetailsForClass(targetClassSymbol, type)}");
            }
            else
            {
                str.AppendLine($"        public {property.ToPropertyDetailsForClass(targetClassSymbol)}");
            }*/

            str.AppendLine($"        public {propertyType} {propertyName} {getSet}");
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