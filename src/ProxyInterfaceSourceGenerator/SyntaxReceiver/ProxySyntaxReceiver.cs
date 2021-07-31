﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProxyInterfaceSourceGenerator.Extensions;

namespace ProxyInterfaceSourceGenerator.SyntaxReceiver
{
    internal class ProxySyntaxReceiver : ISyntaxReceiver
    {
        private static readonly string[] Modifiers = new[] { "public", "partial" };

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
            data = new(string.Empty, string.Empty, string.Empty, string.Empty, false);

            if (interfaceDeclarationSyntax.Modifiers.Select(m => m.ToString()).Except(Modifiers).Count() != 0)
            {
                // InterfaceDeclarationSyntax should be "public" and "partial"
                return false;
            }

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

            string ns = string.Empty;
            if (SyntaxNodeUtils.TryGetParentSyntax(interfaceDeclarationSyntax, out NamespaceDeclarationSyntax namespaceDeclarationSyntax))
            {
                ns = namespaceDeclarationSyntax.Name.ToString();
            }

            string rawTypename = ((TypeOfExpressionSyntax)argumentList.Arguments[0].Expression).Type.ToString();

            data = new
            (
                ns,
                interfaceDeclarationSyntax.Identifier.ToString(),
                rawTypename,
                ResolveType(rawTypename),
                false //bool.Parse(argumentList.Arguments[1].Expression.GetText().ToString())
            );

            return true;
        }

        private static string ResolveType(string typeName)
        {
            int number = typeName.Count(c => c == ',') + 1;
            return $"{typeName.Replace("<", string.Empty).Replace(">", string.Empty)}`{number}";
        }
    }
}