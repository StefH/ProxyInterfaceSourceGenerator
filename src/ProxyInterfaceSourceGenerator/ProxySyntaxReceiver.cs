using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ProxyInterfaceSourceGenerator
{
    internal class ProxySyntaxReceiver : ISyntaxReceiver
    {
        public IDictionary<InterfaceDeclarationSyntax, string> CandidateInterfaces { get; } = new Dictionary<InterfaceDeclarationSyntax, string>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is InterfaceDeclarationSyntax interfaceDeclarationSyntax && TryGet(interfaceDeclarationSyntax, out string typeName))
            {
                CandidateInterfaces.Add(interfaceDeclarationSyntax, typeName);
            }
        }

        private static bool TryGet(InterfaceDeclarationSyntax interfaceDeclarationSyntax, out string typeName)
        {
            typeName = null;

            // TODO : how to check if the InterfaceDeclarationSyntax has 'partial' ?
            var attrinbuteLists = interfaceDeclarationSyntax.AttributeLists.FirstOrDefault(x => x.Attributes.Any(a => a.Name.ToString().Equals("ProxyInterfaceGenerator.Proxy")));
            if (attrinbuteLists == null)
            {
                return false;
            }

            var attributeSyntax = attrinbuteLists.Attributes.FirstOrDefault();
            if (attributeSyntax == null)
            {
                return false;
            }

            var arg = attributeSyntax.ArgumentList.Arguments.First();
            typeName = arg.Expression.ChildNodes().First().GetText().ToString();
            return true;
        }
    }
}
