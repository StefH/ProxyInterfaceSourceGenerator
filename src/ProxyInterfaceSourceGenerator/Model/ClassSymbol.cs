using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace ProxyInterfaceSourceGenerator.Model
{
    internal record ClassSymbol(INamedTypeSymbol Symbol, List<INamedTypeSymbol> BaseTypes)
    {
        public override string ToString()
        {
            return Symbol.ToString();
        }
    }
}