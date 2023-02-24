using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProxyInterfaceSourceGenerator.Types;

namespace ProxyInterfaceSourceGenerator.SyntaxReceiver;

internal static class AttributeArgumentListParser
{
    public static ProxyInterfaceGeneratorAttributeArguments ParseAttributeArguments(AttributeArgumentListSyntax? argumentList)
    {
        var result = new ProxyInterfaceGeneratorAttributeArguments();

        if (argumentList is null || argumentList.Arguments.Count is < 1 or > 3)
        {
            throw new ArgumentException("The ProxyAttribute requires 1, 2 or 3 arguments.");
        }

        if (argumentList.Arguments.Count == 1)
        {
            //if (TryParseAsBoolean(argumentList.Arguments[0].Expression, out var handleBaseClasses))
            //{
            //    return result with { HandleBaseClasses = handleBaseClasses };
            //}

            if (TryParseAsType(argumentList.Arguments[0].Expression, out var rawTypeValue))
            {
                return result with { RawTypeName = rawTypeValue };
            }

            //if (TryParseAsEnum<ProxyInterfaceGeneratorAccessibility>(argumentList.Arguments[0].Expression, out var accessibility))
            //{
            //    return result with { Accessibility = accessibility };
            //}

            throw new ArgumentException("When the ProxyAttribute is used with 1 argument, the only argument should be a Type.");
        }

        foreach (var argument in argumentList.Arguments)
        {
            if (TryParseAsType(argument.Expression, out var rawTypeValue))
            {
                result = result with { RawTypeName = rawTypeValue };
            }
            else
            {
                throw new ArgumentException("When the ProxyAttribute is used with multiple arguments, the first argument should be a Type.");
            }

            if (TryParseAsBoolean(argument.Expression, out var proxyBaseClasses))
            {
                result = result with { ProxyBaseClasses = proxyBaseClasses };
            }

            if (TryParseAsEnum<ProxyInterfaceGeneratorAccessibility>(argument.Expression, out var accessibility))
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

    private static bool TryParseAsType(ExpressionSyntax expressionSyntax, [NotNullWhen(true)] out string? rawTypeName)
    {
        rawTypeName = null;

        if (expressionSyntax is TypeOfExpressionSyntax typeOfExpressionSyntax)
        {
            rawTypeName = typeOfExpressionSyntax.Type.ToString();
            return true;
        }

        return false;
    }

    private static bool TryParseAsEnum<TEnum>(ExpressionSyntax expressionSyntax, out TEnum value)
        where TEnum : struct
    {
        return Enum.TryParse(expressionSyntax.ToString().Substring(typeof(TEnum).Name.Length + 1), out value);
    }
}