using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProxyInterfaceSourceGenerator.Extensions;
using ProxyInterfaceSourceGenerator.Types;

namespace ProxyInterfaceSourceGenerator.SyntaxReceiver;

internal static class AttributeArgumentListParser
{
    private static readonly Regex ProxyAttributesRegex = new(@"^ProxyInterfaceGenerator\.Proxy|Proxy(?:<([^>]+)>)?$", RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    public static bool IsMatch(AttributeSyntax attributeSyntax)
    {
        return ProxyAttributesRegex.IsMatch(attributeSyntax.Name.ToString());
    }

    public static ProxyInterfaceGeneratorAttributeArguments Parse(AttributeSyntax? attributeSyntax, SemanticModel semanticModel)
    {
        if (attributeSyntax == null)
        {
            throw new ArgumentNullException(nameof(attributeSyntax));
        }

        int skip = 0;
        ProxyInterfaceGeneratorAttributeArguments? result;
        if (TryParseAsType(attributeSyntax.Name, semanticModel, out var infoGeneric))
        {
            result = new ProxyInterfaceGeneratorAttributeArguments(infoGeneric.Value.FullyQualifiedDisplayString, infoGeneric.Value.MetadataName);
        }
        else if (attributeSyntax.ArgumentList == null || attributeSyntax.ArgumentList.Arguments.Count is < 1 or > 4)
        {
            throw new ArgumentException("The ProxyAttribute requires 1, 2, 3 or 4 arguments.");
        }
        else if (TryParseAsType(attributeSyntax.ArgumentList.Arguments[0].Expression, semanticModel, out var info))
        {
            skip = 1;
            result = new ProxyInterfaceGeneratorAttributeArguments(info.Value.FullyQualifiedDisplayString, info.Value.MetadataName);
        }
        else
        {
            throw new ArgumentException("The first argument from the ProxyAttribute should be a Type.");
        }

        var array = attributeSyntax.ArgumentList?.Arguments.ToArray() ?? Array.Empty<AttributeArgumentSyntax>();

        foreach (var argument in array.Skip(skip))
        {
            if (TryParseAsStringArray(argument.Expression, out var membersToIgnore))
            {
                result = result with { MembersToIgnore = membersToIgnore };
                continue;
            }

            if (TryParseAsBoolean(argument.Expression, out var proxyBaseClasses))
            {
                result = result with { ProxyBaseClasses = proxyBaseClasses };
                continue;
            }

            if (TryParseAsEnum<ProxyClassAccessibility>(argument.Expression, out var accessibility))
            {
                result = result with { Accessibility = accessibility };
            }
        }

        return result;
    }

    private static bool TryParseAsStringArray(ExpressionSyntax expressionSyntax, [NotNullWhen(true)] out string[]? value)
    {
        if (expressionSyntax is ImplicitArrayCreationExpressionSyntax implicitArrayCreationExpressionSyntax)
        {
            var strings = new List<string>();
            foreach (var expression in implicitArrayCreationExpressionSyntax.Initializer.Expressions)
            {
                if (expression.GetFirstToken().Value is string s)
                {
                    strings.Add(s);
                }
            }
            value = strings.ToArray();
            return true;
        }

        value = default;
        return false;
    }

    private static bool TryParseAsBoolean(ExpressionSyntax expressionSyntax, out bool value)
    {
        value = default;

        if (expressionSyntax is LiteralExpressionSyntax literalExpressionSyntax)
        {
            value = literalExpressionSyntax.Kind() == SyntaxKind.TrueLiteralExpression;
            return true;
        }

        return false;
    }

    private static bool TryParseAsType(
        CSharpSyntaxNode? expressionSyntax,
        SemanticModel semanticModel,
        [NotNullWhen(true)] out (string FullyQualifiedDisplayString, string MetadataName, bool IsGeneric)? info
    )
    {
        info = null;

        bool isGeneric;
        TypeSyntax typeSyntax;
        switch (expressionSyntax)
        {
            case TypeOfExpressionSyntax typeOfExpressionSyntax:
                typeSyntax = typeOfExpressionSyntax.Type;
                isGeneric = false;
                break;

            case QualifiedNameSyntax { Right: GenericNameSyntax genericRightNameSyntax }:
                typeSyntax = genericRightNameSyntax.TypeArgumentList.Arguments.First();
                isGeneric = true;
                break;

            default:
                return false;
        }

        var typeInfo = semanticModel.GetTypeInfo(typeSyntax);
        var typeSymbol = typeInfo.Type!;

        info = new(typeSymbol.ToFullyQualifiedDisplayString(), typeSymbol.GetFullMetadataName(), isGeneric);

        return true;
    }

    private static bool TryParseAsEnum<TEnum>(ExpressionSyntax expressionSyntax, out TEnum value)
        where TEnum : struct
    {
        var enumAsString = expressionSyntax.ToString();
        if (enumAsString.Length > typeof(TEnum).Name.Length && Enum.TryParse(expressionSyntax.ToString().Substring(typeof(TEnum).Name.Length + 1), out value))
        {
            return true;
        }

        value = default;
        return false;
    }
}