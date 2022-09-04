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

        var typeSyntax = ((TypeOfExpressionSyntax)argumentList.Arguments[0].Expression).Type;
        string rawTypeName = typeSyntax.ToString();

        bool proxyBaseClasses;
        try
        {
            proxyBaseClasses = bool.Parse(((LiteralExpressionSyntax)argumentList.Arguments[1].Expression).ToString());
        }
        catch
        {
            proxyBaseClasses = false;
        }

        data = new ProxyData
        {
            Namespace = ns,
            ShortInterfaceName = interfaceDeclarationSyntax.Identifier.ToString(),
            FullInterfaceName = CreateFullInterfaceName(ns, interfaceDeclarationSyntax), // $"{ns}.{interfaceDeclarationSyntax.Identifier}",
            FullRawTypeName = rawTypeName,
            ShortTypeName = ConvertTypeName(rawTypeName).Split('.').Last(),
            FullTypeName = ConvertTypeName(rawTypeName),
            Usings = usings,
            ProxyBaseClasses = proxyBaseClasses
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