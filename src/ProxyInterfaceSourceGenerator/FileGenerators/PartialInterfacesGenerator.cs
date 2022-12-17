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
        foreach (var ci in Context.Candidates)
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
            CreatePartialInterfaceCode(pd.Namespace, targetClassSymbol, interfaceName, pd)
        );

        return true;
    }

    private string CreatePartialInterfaceCode(
        string ns,
        ClassSymbol classSymbol,
        string interfaceName,
        ProxyData proxyData)
    {
        var extendsProxyClasses = GetExtendsProxyData(proxyData, classSymbol);
        var @new = extendsProxyClasses.Any() ? "new " : string.Empty;
        var (namespaceStart, namespaceEnd) = NamespaceBuilder.Build(ns);

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

{namespaceStart}
    public partial interface {interfaceName}
    {{
        {@new}{classSymbol.Symbol} _Instance {{ get; }}

{GenerateProperties(classSymbol, proxyData.ProxyBaseClasses)}

{GenerateMethods(classSymbol, proxyData.ProxyBaseClasses)}

{GenerateEvents(classSymbol, proxyData.ProxyBaseClasses)}
    }}
{namespaceEnd}
{(SupportsNullable ? "#nullable disable" : string.Empty)}";
    }

    private string GenerateProperties(ClassSymbol targetClassSymbol, bool proxyBaseClasses)
    {
        var str = new StringBuilder();

        foreach (var property in MemberHelper.GetPublicProperties(targetClassSymbol, proxyBaseClasses))
        {
            var type = GetPropertyType(property, out var isReplaced);

            var getterSetter = isReplaced ? property.ToPropertyDetails(type) : property.ToPropertyDetails();
            if (getterSetter is null)
            {
                continue;
            }

            var propertyName = getterSetter.Value.PropertyName;

            if (property.IsIndexer)
            {
                var methodParameters = GetMethodParameters(property.Parameters, true);
                propertyName = $"this[{string.Join(", ", methodParameters)}]";
            }

            str.AppendLine($"        {getterSetter.Value.PropertyType} {propertyName} {getterSetter.Value.GetSet}");
            str.AppendLine();
        }

        return str.ToString();
    }

    private string GenerateMethods(ClassSymbol targetClassSymbol, bool proxyBaseClasses)
    {
        var str = new StringBuilder();
        foreach (var method in MemberHelper.GetPublicMethods(targetClassSymbol, proxyBaseClasses))
        {
            var methodParameters = GetMethodParameters(method.Parameters, true);
            var whereStatement = GetWhereStatementFromMethod(method);

            foreach (var attribute in method.GetAttributesAsList())
            {
                str.AppendLine($"        {attribute}");
            }

            str.AppendLine($"        {GetReplacedType(method.ReturnType, out _)} {method.GetMethodNameWithOptionalTypeParameters()}({string.Join(", ", methodParameters)}){whereStatement};");
            str.AppendLine();
        }

        return str.ToString();
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