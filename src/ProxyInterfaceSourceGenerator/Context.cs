using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProxyInterfaceSourceGenerator.SyntaxReceiver;

namespace ProxyInterfaceSourceGenerator
{
    internal record Context
    {
        public GeneratorExecutionContext GeneratorExecutionContext { get; init; }

        public List<ContextData> GeneratedData { get; } = new List<ContextData>();

        public IDictionary<InterfaceDeclarationSyntax, ProxyData> CandidateInterfaces { get; init; }

        public Dictionary<string, string> ReplacedTypes { get; } = new Dictionary<string, string>();
}
}