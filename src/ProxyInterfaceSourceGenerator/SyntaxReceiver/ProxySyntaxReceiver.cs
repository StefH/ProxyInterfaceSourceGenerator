using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ProxyInterfaceSourceGenerator.SyntaxReceiver
{
    internal class ProxySyntaxReceiver : ISyntaxReceiver
    {
        public IDictionary<InterfaceDeclarationSyntax, ProxyData> CandidateInterfaces { get; } = new Dictionary<InterfaceDeclarationSyntax, ProxyData>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is InterfaceDeclarationSyntax interfaceDeclarationSyntax && TryGet(interfaceDeclarationSyntax, out var data))
            {
                CandidateInterfaces.Add(interfaceDeclarationSyntax, data);
            }
        }

        private static bool TryGet(InterfaceDeclarationSyntax interfaceDeclarationSyntax, out ProxyData data)
        {
            data = new(string.Empty, string.Empty, false);

            // TODO : how to check if the InterfaceDeclarationSyntax has 'partial' ?
            var attributeLists = interfaceDeclarationSyntax.AttributeLists.FirstOrDefault(x => x.Attributes.Any(a => a.Name.ToString().Equals("ProxyInterfaceGenerator.Proxy")));
            if (attributeLists is null)
            {
                return false;
            }

            var argumentList = attributeLists.Attributes.FirstOrDefault()?.ArgumentList;
            if (argumentList is null)
            {
                return false;
            }

            data = new
            (
                interfaceDeclarationSyntax.Identifier.ToString(),
                argumentList.Arguments[0].Expression.ChildNodes().First().GetText().ToString(),
                false //bool.Parse(argumentList.Arguments[1].Expression.GetText().ToString())
            );

            return true;
        }
    }
}