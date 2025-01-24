using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ProxyInterfaceSourceGenerator.Models;

internal record Context
{
    public GeneratorExecutionContext GeneratorExecutionContext { get; init; }

    public IDictionary<InterfaceDeclarationSyntax, ProxyData> Candidates { get; init; } = default!;

    public List<ReplacedTypeInfo> ReplacedTypes { get; } = new();
}