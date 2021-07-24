using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProxyInterfaceSourceGenerator.SyntaxReceiver;

namespace ProxyInterfaceSourceGenerator.FileGenerators
{
    internal abstract class BaseGenerator
    {
        protected readonly Context _context;
        protected readonly IDictionary<InterfaceDeclarationSyntax, ProxyData> _candidateInterfaces;

        public BaseGenerator(Context context, IDictionary<InterfaceDeclarationSyntax, ProxyData> candidateInterfaces)
        {
            _context = context;
            _candidateInterfaces = candidateInterfaces;
        }

        protected INamedTypeSymbol GetType(string name)
        {
            var symbol = _context.GeneratorExecutionContext.Compilation.GetTypeByMetadataName(name);
            if (symbol is null)
            {
                throw new Exception($"The type '{name}' is not found.");
            }

            return symbol;
        }
    }
}