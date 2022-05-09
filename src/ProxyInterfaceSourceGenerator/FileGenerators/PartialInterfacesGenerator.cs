using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProxyInterfaceSourceGenerator.Enums;
using ProxyInterfaceSourceGenerator.Extensions;
using ProxyInterfaceSourceGenerator.Models;
using ProxyInterfaceSourceGenerator.Utils;

namespace ProxyInterfaceSourceGenerator.FileGenerators;

internal class PartialInterfacesGenerator : BaseGenerator, IFilesGenerator
{
    public PartialInterfacesGenerator(Context context, bool supportsNullable) :
        base(context, supportsNullable)
    {
    }

    public IEnumerable<FileData> GenerateFiles()
    {
        foreach (var ci in Context.CandidateInterfaces)
        {
            if (TryGenerateFile(ci.Key, ci.Value, out var file))
            {
                yield return file;
            }
        }
    }

    private bool TryGenerateFile(InterfaceDeclarationSyntax ci, ProxyData pd, [NotNullWhen(true)] out FileData? fileData)
    {
        fileData = default;

        if (!TryGetNamedTypeSymbolByFullName(TypeKind.Interface, ci.Identifier.ToString(), pd.Usings, out var sourceInterfaceSymbol))
        {
            return false;
        }

        if (!TryGetNamedTypeSymbolByFullName(TypeKind.Class, pd.FullTypeName, pd.Usings, out var targetClassSymbol))
        {
            return false;
        }

        var interfaceName = ResolveInterfaceNameWithOptionalTypeConstraints(targetClassSymbol.Symbol, pd.ShortInterfaceName);

        fileData = new FileData(
            $"{sourceInterfaceSymbol.Symbol.GetFileName()}.g.cs",
            CreatePartialInterfaceCode(pd.Namespace, targetClassSymbol, interfaceName, pd.ProxyBaseClasses)
        );

        return true;
    }

    private string CreatePartialInterfaceCode(
        string ns,
        ClassSymbol classSymbol,
        string interfaceName,
        bool proxyBaseClasses) => $@"//----------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by https://github.com/StefH/ProxyInterfaceSourceGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//----------------------------------------------------------------------------------------

{(SupportsNullable ? "#nullable enable" : string.Empty)}
using System;

namespace {ns}
{{
    public partial interface {interfaceName}
    {{
{GenerateProperties(classSymbol, proxyBaseClasses)}

{GenerateMethods(classSymbol, proxyBaseClasses)}

{GenerateEvents(classSymbol, proxyBaseClasses)}
    }}
}}
{(SupportsNullable ? "#nullable disable" : string.Empty)}";

    private string GenerateProperties(ClassSymbol targetClassSymbol, bool proxyBaseClasses)
    {
        var str = new StringBuilder();

        foreach (var property in MemberHelper.GetPublicProperties(targetClassSymbol, proxyBaseClasses))
        {
            var type = GetPropertyType(property, out var isReplaced);

            (string PropertyType, string? PropertyName, string GetSet) = isReplaced ?
                property.ToPropertyDetails(type) :
                property.ToPropertyDetails();

            if (property.IsIndexer)
            {
                var methodParameters = GetMethodParameters(property.Parameters.ToArray());
                PropertyName = $"this[{string.Join(",", methodParameters)}]";
            }
            
            str.AppendLine($"        {PropertyType} {PropertyName} {GetSet}");
            str.AppendLine();
        }

        return str.ToString();
    }

    private string GenerateMethods(ClassSymbol targetClassSymbol, bool proxyBaseClasses)
    {
        var str = new StringBuilder();
        foreach (var method in MemberHelper.GetPublicMethods(targetClassSymbol, proxyBaseClasses))
        {
            var methodParameters = GetMethodParameters(method.Parameters.ToArray());
            //var methodParameters = new List<string>();
            //foreach (var ps in method.Parameters)
            //{
            //    var type = ps.GetTypeEnum() == TypeEnum.Complex ? GetParameterType(ps, out _) : ps.Type.ToString();
            //    methodParameters.Add($"{ps.GetParamsPrefix()}{ps.GetRefPrefix()}{type} {ps.GetSanitizedName()}{ps.GetDefaultValue()}");
            //}

            var whereStatement = GetWhereStatementFromMethod(method);

            str.AppendLine($"        {GetReplacedType(method.ReturnType, out _)} {method.GetMethodNameWithOptionalTypeParameters()}({string.Join(", ", methodParameters)}){whereStatement};");
            str.AppendLine();
        }

        return str.ToString();
    }

    private IList<string> GetMethodParameters(IParameterSymbol[] parameters)
    {
        var methodParameters = new List<string>();
        foreach (var ps in parameters)
        {
            var type = ps.GetTypeEnum() == TypeEnum.Complex ? GetParameterType(ps, out _) : ps.Type.ToString();
            methodParameters.Add($"{ps.GetParamsPrefix()}{ps.GetRefPrefix()}{type} {ps.GetSanitizedName()}{ps.GetDefaultValue()}");
        }

        return methodParameters;
    }

    private string GenerateEvents(ClassSymbol targetClassSymbol, bool proxyBaseClasses)
    {
        var str = new StringBuilder();
        foreach (var @event in MemberHelper.GetPublicEvents(targetClassSymbol, proxyBaseClasses))
        {
            var ps = @event.First().Parameters.First();
            var type = ps.GetTypeEnum() == TypeEnum.Complex ? GetParameterType(ps, out _) : ps.Type.ToString();
            str.AppendLine($"        event {type} {@event.Key.GetSanitizedName()};");
            str.AppendLine();
        }

        return str.ToString();
    }
}