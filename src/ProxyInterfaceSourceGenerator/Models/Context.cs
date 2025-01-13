using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ProxyInterfaceSourceGenerator.Models;

internal record Context
{
    public GeneratorExecutionContext GeneratorExecutionContext { get; init; }

    public required IDictionary<InterfaceDeclarationSyntax, ProxyData> Candidates { get; init; }

    public Dictionary<string, string> DirectReplacedTypes { get; } = new();

    public Dictionary<string, string> IndirectReplacedTypes { get; } = new();
}