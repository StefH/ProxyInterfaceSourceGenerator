using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProxyInterfaceSourceGenerator.Extensions;
using ProxyInterfaceSourceGenerator.Models;

namespace ProxyInterfaceSourceGenerator.SyntaxReceiver;

internal class ProxySyntaxReceiver : ISyntaxReceiver
{
    private static readonly string[] Modifiers = { "public", "partial" };
    private static readonly string[] GenerateProxyAttributes = { "ProxyInterfaceGenerator.Proxy", "Proxy" };

    public IDictionary<InterfaceDeclarationSyntax, ProxyData> CandidateInterfaces { get; } = new Dictionary<InterfaceDeclarationSyntax, ProxyData>();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is InterfaceDeclarationSyntax interfaceDeclarationSyntax && TryGet(interfaceDeclarationSyntax, out var data))
        {
            CandidateInterfaces.Add(interfaceDeclarationSyntax, data);
        }
    }

    private static bool TryGet(InterfaceDeclarationSyntax interfaceDeclarationSyntax, [NotNullWhen(true)] out ProxyData? data)
    {
        data = null;

        if (interfaceDeclarationSyntax.Modifiers.Select(m => m.ToString()).Except(Modifiers).Count() != 0)
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

        var fluentBuilderAttributeArguments = AttributeArgumentListParser.ParseAttributeArguments(attributeList.Attributes.FirstOrDefault()?.ArgumentList);

        var rawTypeNameAsString = fluentBuilderAttributeArguments.RawTypeName;
        
        data = new ProxyData
        {
            Namespace = ns,
            ShortInterfaceName = interfaceDeclarationSyntax.Identifier.ToString(),
            FullInterfaceName = CreateFullInterfaceName(ns, interfaceDeclarationSyntax), // $"{ns}.{interfaceDeclarationSyntax.Identifier}",
            FullRawTypeName = rawTypeNameAsString,
            ShortTypeName = ConvertTypeName(rawTypeNameAsString).Split('.').Last(),
            FullTypeName = ConvertTypeName(rawTypeNameAsString),
            Usings = usings,
            ProxyBaseClasses = fluentBuilderAttributeArguments.ProxyBaseClasses,
            Accessibility = fluentBuilderAttributeArguments.Accessibility
        };

        return true;
    }

    private static string ConvertTypeName(string typeName)
    {
        return !(typeName.Contains('<') && typeName.Contains('>')) ?
            typeName :
            $"{typeName.Replace("<", string.Empty).Replace(">", string.Empty).Replace(",", string.Empty).Trim()}`{typeName.Count(c => c == ',') + 1}";
    }

    private static string CreateFullInterfaceName(string ns, BaseTypeDeclarationSyntax classDeclarationSyntax)
    {
        return !string.IsNullOrEmpty(ns) ? $"{ns}.{classDeclarationSyntax.Identifier}" : classDeclarationSyntax.Identifier.ToString();
    }
}