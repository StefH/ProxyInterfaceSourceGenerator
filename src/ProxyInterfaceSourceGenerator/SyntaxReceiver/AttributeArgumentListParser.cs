using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProxyInterfaceSourceGenerator.Extensions;
using ProxyInterfaceSourceGenerator.Types;

namespace ProxyInterfaceSourceGenerator.SyntaxReceiver;

internal static class AttributeArgumentListParser
{
    public static ProxyInterfaceGeneratorAttributeArguments ParseAttributeArguments(AttributeArgumentListSyntax? argumentList, SemanticModel semanticModel)
    {
        if (argumentList is null || argumentList.Arguments.Count is < 1 or > 3)
        {
            throw new ArgumentException("The ProxyAttribute requires 1, 2 or 3 arguments.");
        }

        ProxyInterfaceGeneratorAttributeArguments result;
        if (TryParseAsType(argumentList.Arguments[0].Expression, out var rawTypeValue, semanticModel))
        {
            result = new ProxyInterfaceGeneratorAttributeArguments(rawTypeValue);
        }
        else
        {
            throw new ArgumentException("The first argument from the ProxyAttribute should be a Type.");
        }

        foreach (var argument in argumentList.Arguments.Skip(1))
        {
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

    private static bool TryParseAsType(ExpressionSyntax expressionSyntax, [NotNullWhen(true)] out string? rawTypeName, SemanticModel semanticModel)
    {
        rawTypeName = null;

        if (expressionSyntax is TypeOfExpressionSyntax typeOfExpressionSyntax)
        {
            var typeInfo = semanticModel.GetTypeInfo(typeOfExpressionSyntax.Type);
            var typeSymbol = typeInfo.Type;
            rawTypeName = typeSymbol!.ToFullyQualifiedDisplayString();

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