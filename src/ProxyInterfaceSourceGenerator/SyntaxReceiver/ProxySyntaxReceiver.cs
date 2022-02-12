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

        var attributeLists = interfaceDeclarationSyntax.AttributeLists.FirstOrDefault(x => x.Attributes.Any(a => GenerateProxyAttributes.Contains(a.Name.ToString())));
        if (attributeLists is null)
        {
            // InterfaceDeclarationSyntax should have the correct attribute
            return false;
        }

        var argumentList = attributeLists.Attributes.FirstOrDefault()?.ArgumentList;
        if (argumentList is null)
        {
            return false;
        }

        var usings = new List<string>();

        string ns = string.Empty;
        if (interfaceDeclarationSyntax.TryGetParentSyntax(out NamespaceDeclarationSyntax? namespaceDeclarationSyntax))
        {
            ns = namespaceDeclarationSyntax.Name.ToString();
            usings.Add(ns);
        }

        if (interfaceDeclarationSyntax.TryGetParentSyntax(out CompilationUnitSyntax? cc))
        {
            foreach (var @using in cc.Usings)
            {
                usings.Add(@using.Name.ToString());
            }
        }

        var typeSyntax = ((TypeOfExpressionSyntax)argumentList.Arguments[0].Expression).Type;
        string rawTypeName = typeSyntax.ToString();

        bool proxyAllClasses;
        try
        {
            proxyAllClasses = bool.Parse(((LiteralExpressionSyntax)argumentList.Arguments[1].Expression).ToString());
        }
        catch
        {
            proxyAllClasses = false;
        }
        
        data = new
        (
            ns,
            interfaceDeclarationSyntax.Identifier.ToString(),
            $"{ns}.{interfaceDeclarationSyntax.Identifier}",
            rawTypeName,
            ConvertTypeName(rawTypeName).Split('.').Last(), // ShortTypeName
            ConvertTypeName(rawTypeName), // FullTypeName
            usings,
            proxyAllClasses
        );

        return true;
    }

    private static string ConvertTypeName(string typeName)
    {
        return !(typeName.Contains('<') && typeName.Contains('>')) ?
            typeName :
            $"{typeName.Replace("<", string.Empty).Replace(">", string.Empty).Replace(",", string.Empty).Trim()}`{typeName.Count(c => c == ',') + 1}";
    }
}