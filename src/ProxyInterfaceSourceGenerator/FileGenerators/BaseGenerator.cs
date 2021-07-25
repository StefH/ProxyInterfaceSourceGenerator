using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace ProxyInterfaceSourceGenerator.FileGenerators
{
    internal abstract class BaseGenerator
    {
        protected readonly Context _context;

        public BaseGenerator(Context context)
        {
            _context = context;
        }

        protected string GetPropertyType(IPropertySymbol property, out Dictionary<string, string> differs)
        {
            differs = new Dictionary<string, string>();

            var existing = _context.CandidateInterfaces.Values.FirstOrDefault(x => x.TypeName == property.Type.ToString());
            if (existing is not null)
            {
                differs.Add(property.Type.ToString(), existing.InterfaceName);
                return existing.InterfaceName;
            }

            if (property.Type is INamedTypeSymbol namedTypedSymbol)
            {
                var type = property.Type.ToString();
                foreach (var typeArgument in namedTypedSymbol.TypeArguments)
                {
                    var exist = _context.CandidateInterfaces.Values.FirstOrDefault(x => x.TypeName == typeArgument.ToString());

                    if (exist is not null)
                    {
                        if (!differs.ContainsKey(typeArgument.ToString()))
                        {
                            differs.Add(typeArgument.ToString(), exist.InterfaceName);
                        }

                        type = type.Replace(typeArgument.ToString(), exist.InterfaceName);
                    }
                }

                return type;
            }

            return property.Type.ToString();
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