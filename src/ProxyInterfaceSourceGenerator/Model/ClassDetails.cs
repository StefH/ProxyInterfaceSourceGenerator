using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace ProxyInterfaceSourceGenerator.Model
{
    internal record ClassDetails(INamedTypeSymbol Symbol, List<INamedTypeSymbol> BaseTypes);
}