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
    private static readonly Regex ProxyAttributesRegex = new("^(ProxyInterfaceGenerator.Proxy|Proxy)(?:<([^>]+)>)?$", RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    public static bool IsMatch(AttributeSyntax attributeSyntax)
    {
        return ProxyAttributesRegex.IsMatch(attributeSyntax.Name.ToString());
    }

    public static ProxyInterfaceGeneratorAttributeArguments ParseAttributeArguments(AttributeArgumentListSyntax? argumentList, SemanticModel semanticModel)
    {
        if (argumentList is null || argumentList.Arguments.Count is < 1 or > 4)
        {
            throw new ArgumentException("The ProxyAttribute requires 1, 2, 3 or 4 arguments.");
        }

        ProxyInterfaceGeneratorAttributeArguments result;
        if (TryParseAsType(argumentList.Arguments[0].Expression, semanticModel, out var fullyQualifiedDisplayString, out var metadataName))
        {
            result = new ProxyInterfaceGeneratorAttributeArguments(fullyQualifiedDisplayString, metadataName);
        }
        else
        {
            throw new ArgumentException("The first argument from the ProxyAttribute should be a Type.");
        }

        foreach (var argument in argumentList.Arguments.Skip(1))
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

    private static bool TryParseAsType(ExpressionSyntax expressionSyntax, SemanticModel semanticModel, [NotNullWhen(true)] out string? fullyQualifiedDisplayString, [NotNullWhen(true)] out string? metadataName)
    {
        fullyQualifiedDisplayString = null;
        metadataName = null;

        if (expressionSyntax is TypeOfExpressionSyntax typeOfExpressionSyntax)
        {
            var typeInfo = semanticModel.GetTypeInfo(typeOfExpressionSyntax.Type);
            var typeSymbol = typeInfo.Type!;
            metadataName = typeSymbol.GetFullMetadataName();
            fullyQualifiedDisplayString = typeSymbol.ToFullyQualifiedDisplayString();

            return true;
        }

        return false;
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