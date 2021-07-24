using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace ProxyInterfaceSourceGenerator
{
    internal record Context
    {
        public GeneratorExecutionContext GeneratorExecutionContext { get; init; }

        public List<ContextData> GeneratedData { get; } = new List<ContextData>();
    }
}