using System;
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

        protected string GetPropertyType(IPropertySymbol property, out bool isReplaced)
        {
            return GetReplacedType(property.Type, out isReplaced);
        }

        protected string GetParameterType(IParameterSymbol property, out bool isReplaced)
        {
            return GetReplacedType(property.Type, out isReplaced);
        }

        protected string GetReplacedType(ITypeSymbol typeSymbol, out bool isReplaced)
        {
            isReplaced = false;

            var typeSymbolAsString = typeSymbol.ToString();

            var existing = _context.CandidateInterfaces.Values.FirstOrDefault(x => x.RawTypeName == typeSymbolAsString);
            if (existing is not null)
            {
                if (!_context.ReplacedTypes.ContainsKey(typeSymbolAsString))
                {
                    _context.ReplacedTypes.Add(typeSymbolAsString, existing.InterfaceName);
                }

                isReplaced = true;
                return existing.InterfaceName;
            }

            if (typeSymbol is INamedTypeSymbol namedTypedSymbol)
            {
                var propertyTypeAsStringToBeModified = typeSymbolAsString;
                foreach (var typeArgument in namedTypedSymbol.TypeArguments)
                {
                    var typeArgumentAsString = typeArgument.ToString();
                    var exist = _context.CandidateInterfaces.Values.FirstOrDefault(x => x.RawTypeName == typeArgumentAsString);
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

        protected INamedTypeSymbol GetNamedTypeSymbolByFullName(string fullName)
        {
            // The GetTypeByMetadataName method returns null if no type matches the full name or if 2 or more types (in different assemblies) match the full name.
            var symbol = _context.GeneratorExecutionContext.Compilation.GetTypeByMetadataName(fullName);
            if (symbol is null)
            {
                throw new Exception($"The type '{fullName}' is not found.");
            }

            return symbol;
        }
    }
}