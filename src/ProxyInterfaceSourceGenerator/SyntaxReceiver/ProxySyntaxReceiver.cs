using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProxyInterfaceSourceGenerator.Extensions;
using ProxyInterfaceSourceGenerator.Models;

namespace ProxyInterfaceSourceGenerator.SyntaxReceiver;

internal class ProxySyntaxReceiver : ISyntaxContextReceiver
{
    private const string GlobalPrefix = "global::";
    private static readonly string[] GenerateProxyAttributes = { "ProxyInterfaceGenerator.Proxy", "Proxy" };
    private static readonly string[] Modifiers = { "public", "partial" };
    public IDictionary<InterfaceDeclarationSyntax, ProxyData> CandidateInterfaces { get; } = new Dictionary<InterfaceDeclarationSyntax, ProxyData>();

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        var syntaxNode = context.Node;
        var semanticModel = context.SemanticModel;

        if (syntaxNode is InterfaceDeclarationSyntax interfaceDeclarationSyntax && TryGet(interfaceDeclarationSyntax, out var data, semanticModel!))
        {
            CandidateInterfaces.Add(interfaceDeclarationSyntax, data);
        }
    }

    private static string CreateFullInterfaceName(string ns, BaseTypeDeclarationSyntax classDeclarationSyntax)
    {
        return !string.IsNullOrEmpty(ns) ? $"{ns}.{classDeclarationSyntax.Identifier}" : classDeclarationSyntax.Identifier.ToString();
    }
    private static bool TryGet(InterfaceDeclarationSyntax interfaceDeclarationSyntax, [NotNullWhen(true)] out ProxyData? data, SemanticModel semanticModel)
    {
        data = null;

        if (interfaceDeclarationSyntax.Modifiers.Select(m => m.ToString()).Except(Modifiers).Any())
        {
            // InterfaceDeclarationSyntax should be "public" and "partial"
            return false;
        }

        var attributeList = interfaceDeclarationSyntax.AttributeLists.FirstOrDefault(x => x.Attributes.Any(a => GenerateProxyAttributes.Contains(a.Name.ToString())));
        if (attributeList is null)
        {
            // InterfaceDeclarationSyntax should have the correct attribute
            return false;
        }

        var usings = new List<string>();

        string ns = interfaceDeclarationSyntax.GetNamespace();
        if (!string.IsNullOrEmpty(ns))
        {
            usings.Add(ns);
        }

        if (interfaceDeclarationSyntax.TryGetParentSyntax(out CompilationUnitSyntax? cc))
        {
            foreach (var @using in cc.Usings)
            {
                usings.Add(@using.Name.ToString());
            }
        }

        var fluentBuilderAttributeArguments = AttributeArgumentListParser.ParseAttributeArguments(attributeList.Attributes.FirstOrDefault()?.ArgumentList, semanticModel);

        var metadataName = fluentBuilderAttributeArguments.MetadataName;
        var globalNamespace = string.IsNullOrEmpty(ns) ? string.Empty : $"{GlobalPrefix}{ns}";
        var namespaceDot = string.IsNullOrEmpty(ns) ? string.Empty : $"{ns}.";

        data = new ProxyData(
            @namespace: ns,
            namespaceDot: namespaceDot,
            shortInterfaceName: interfaceDeclarationSyntax.Identifier.ToString(),
            fullInterfaceName: CreateFullInterfaceName(globalNamespace, interfaceDeclarationSyntax), // $"{ns}.{interfaceDeclarationSyntax.Identifier}",
            fullQualifiedTypeName: fluentBuilderAttributeArguments.FullyQualifiedDisplayString,
            fullMetadataTypeName: metadataName,
            shortMetadataTypeName: metadataName.Split('.').Last(),
            usings: usings,
            proxyBaseClasses: fluentBuilderAttributeArguments.ProxyBaseClasses,
            accessibility: fluentBuilderAttributeArguments.Accessibility
        );

        return true;
    }
}