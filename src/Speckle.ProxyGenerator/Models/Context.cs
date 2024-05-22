using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Speckle.ProxyGenerator.Models;

internal record Context
{
    public GeneratorExecutionContext GeneratorExecutionContext { get; init; }

    public IDictionary<InterfaceDeclarationSyntax, ProxyData> Candidates { get; init; } = default!;

    public Dictionary<string, string> ReplacedTypes { get; } = new();
}
