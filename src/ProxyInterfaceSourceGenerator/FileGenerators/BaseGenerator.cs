using System;
using System.Linq;
// using AnyOfTypes;
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

        protected string GetPropertyType(IPropertySymbol property, out bool isReplaced)
        {
            return GetReplacedType(property.Type, out isReplaced);
        }

        protected string GetParameterType(IParameterSymbol property, out bool isReplaced)
        {
            return GetReplacedType(property.Type, out isReplaced);
        }

        protected string GetReplacedType(ITypeSymbol property, out bool isReplaced)
        {
            isReplaced = false;

            var typeSymbolAsString = property.ToString();

            var existing = _context.CandidateInterfaces.Values.FirstOrDefault(x => x.TypeName == typeSymbolAsString);
            if (existing is not null)
            {
                if (!_context.ReplacedTypes.ContainsKey(typeSymbolAsString))
                {
                    _context.ReplacedTypes.Add(typeSymbolAsString, existing.InterfaceName);
                }

                isReplaced = true;
                return existing.InterfaceName;
            }

            if (property is INamedTypeSymbol namedTypedSymbol)
            {
                var propertyTypeAsStringToBeModified = typeSymbolAsString;
                foreach (var typeArgument in namedTypedSymbol.TypeArguments)
                {
                    var typeArgumentAsString = typeArgument.ToString();
                    var exist = _context.CandidateInterfaces.Values.FirstOrDefault(x => x.TypeName == typeArgumentAsString);
                    if (exist is not null)
                    {
                        isReplaced = true;

                        if (!_context.ReplacedTypes.ContainsKey(typeArgumentAsString))
                        {
                            _context.ReplacedTypes.Add(typeArgumentAsString, exist.InterfaceName);
                        }

                        propertyTypeAsStringToBeModified = propertyTypeAsStringToBeModified.Replace(typeArgumentAsString, exist.InterfaceName);
                    }
                }

                return propertyTypeAsStringToBeModified;
            }

            
            return typeSymbolAsString;
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