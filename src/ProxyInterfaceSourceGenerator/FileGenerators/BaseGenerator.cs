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

        //protected string GetReplacedType(AnyOf<IPropertySymbol, IParameterSymbol> x)
        //{
        //    ITypeSymbol ts = x.IsFirst ? x.First.Type : x.Second.Type;
        //    var propertyTypeAsString = ts.ToString();

        //    var existing = _context.CandidateInterfaces.Values.FirstOrDefault(x => x.TypeName == propertyTypeAsString);
        //    if (existing is not null)
        //    {
        //        if (!_context.ReplacedTypes.ContainsKey(propertyTypeAsString))
        //        {
        //            _context.ReplacedTypes.Add(propertyTypeAsString, existing.InterfaceName);
        //        }

        //        return existing.InterfaceName;
        //    }

        //    if (ts is INamedTypeSymbol namedTypedSymbol)
        //    {
        //        var propertyTypeAsStringToBeModified = propertyTypeAsString;
        //        foreach (var typeArgument in namedTypedSymbol.TypeArguments)
        //        {
        //            var typeArgumentAsString = typeArgument.ToString();
        //            var exist = _context.CandidateInterfaces.Values.FirstOrDefault(x => x.TypeName == typeArgumentAsString);
        //            if (exist is not null)
        //            {
        //                if (!_context.ReplacedTypes.ContainsKey(typeArgumentAsString))
        //                {
        //                    _context.ReplacedTypes.Add(typeArgumentAsString, exist.InterfaceName);
        //                }

        //                propertyTypeAsStringToBeModified = propertyTypeAsStringToBeModified.Replace(typeArgumentAsString, exist.InterfaceName);
        //            }
        //        }

        //        return propertyTypeAsStringToBeModified;
        //    }

        //    return propertyTypeAsString;
        //}

        protected string GetPropertyType(IPropertySymbol property)
        {
            var propertyTypeAsString = property.Type.ToString();

            var existing = _context.CandidateInterfaces.Values.FirstOrDefault(x => x.TypeName == propertyTypeAsString);
            if (existing is not null)
            {
                if (!_context.ReplacedTypes.ContainsKey(propertyTypeAsString))
                {
                    _context.ReplacedTypes.Add(propertyTypeAsString, existing.InterfaceName);
                }

                return existing.InterfaceName;
            }

            if (property.Type is INamedTypeSymbol namedTypedSymbol)
            {
                var propertyTypeAsStringToBeModified = propertyTypeAsString;
                foreach (var typeArgument in namedTypedSymbol.TypeArguments)
                {
                    var typeArgumentAsString = typeArgument.ToString();
                    var exist = _context.CandidateInterfaces.Values.FirstOrDefault(x => x.TypeName == typeArgumentAsString);
                    if (exist is not null)
                    {
                        if (!_context.ReplacedTypes.ContainsKey(typeArgumentAsString))
                        {
                            _context.ReplacedTypes.Add(typeArgumentAsString, exist.InterfaceName);
                        }

                        propertyTypeAsStringToBeModified = propertyTypeAsStringToBeModified.Replace(typeArgumentAsString, exist.InterfaceName);
                    }
                }

                return propertyTypeAsStringToBeModified;
            }

            return propertyTypeAsString;
        }

        protected string GetParameterType(IParameterSymbol property)
        {
            var propertyTypeAsString = property.Type.ToString();

            var existing = _context.CandidateInterfaces.Values.FirstOrDefault(x => x.TypeName == propertyTypeAsString);
            if (existing is not null)
            {
                if (!_context.ReplacedTypes.ContainsKey(propertyTypeAsString))
                {
                    _context.ReplacedTypes.Add(propertyTypeAsString, existing.InterfaceName);
                }

                return existing.InterfaceName;
            }

            if (property.Type is INamedTypeSymbol namedTypedSymbol)
            {
                var propertyTypeAsStringToBeModified = propertyTypeAsString;
                foreach (var typeArgument in namedTypedSymbol.TypeArguments)
                {
                    var typeArgumentAsString = typeArgument.ToString();
                    var exist = _context.CandidateInterfaces.Values.FirstOrDefault(x => x.TypeName == typeArgumentAsString);
                    if (exist is not null)
                    {
                        if (!_context.ReplacedTypes.ContainsKey(typeArgumentAsString))
                        {
                            _context.ReplacedTypes.Add(typeArgumentAsString, exist.InterfaceName);
                        }

                        propertyTypeAsStringToBeModified = propertyTypeAsStringToBeModified.Replace(typeArgumentAsString, exist.InterfaceName);
                    }
                }

                return propertyTypeAsStringToBeModified;
            }

            return propertyTypeAsString;
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