using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ProxyInterfaceSourceGenerator.Models;

internal record Context
{
    public GeneratorExecutionContext GeneratorExecutionContext { get; init; }

    // public List<ContextData> GeneratedData { get; } = new List<ContextData>();

    public IDictionary<InterfaceDeclarationSyntax, ProxyData> Candidates { get; init; } = default!;

    public Dictionary<string, string> ReplacedTypes { get; } = new();
}