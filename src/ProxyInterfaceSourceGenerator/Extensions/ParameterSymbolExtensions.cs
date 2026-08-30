using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProxyInterfaceSourceGenerator.Enums;

namespace ProxyInterfaceSourceGenerator.Extensions;

internal static class ParameterSymbolExtensions
{
    private const string ParameterValueNull = "null";

    private const string ParameterValueDefault = "default";

    public static bool IsNullable(this IParameterSymbol ps) => ps.Type.NullableAnnotation == NullableAnnotation.Annotated;

    public static bool IsRef(this IParameterSymbol ps)
    {
        return ps.RefKind is RefKind.Ref or RefKind.RefReadOnly;
    }

    public static string GetRefKindPrefix(this IParameterSymbol ps)
    {
        return ps.RefKind switch
        {
            RefKind.In => "in ",
            RefKind.Out => "out ",
            RefKind.Ref => "ref ",
            _ => string.Empty
        };
    }

    public static string GetParamsPrefix(this IParameterSymbol ps) => ps.IsParams ? "params " : string.Empty;

    public static string GetDefaultValue(this IParameterSymbol ps)
    {
        if (!ps.HasExplicitDefaultValue)
        {
            return string.Empty;
        }

        var defaultValueSyntax = GetDefaultValueSyntax(ps);
        if (defaultValueSyntax is not null)
        {
            if (defaultValueSyntax.IsKind(SyntaxKind.DefaultLiteralExpression) ||
                defaultValueSyntax.IsKind(SyntaxKind.DefaultExpression))
            {
                return IsNonNullableReferenceTypeInNullableEnabledContext(ps)
                    ? $" = {ParameterValueDefault}!"
                    : $" = {ParameterValueDefault}";
            }

            if (defaultValueSyntax.IsKind(SyntaxKind.NullLiteralExpression))
            {
                return IsNonNullableReferenceTypeInNullableEnabledContext(ps)
                    ? $" = {ParameterValueNull}!"
                    : $" = {ParameterValueNull}";
            }

            return $" = {defaultValueSyntax.ToString()}";
        }

        if (ps.ExplicitDefaultValue is null)
        {
            if (IsNonNullableReferenceTypeInNullableEnabledContext(ps))
            {
                return $" = {ParameterValueNull}!";
            }

            if (ps.Type.IsValueType && ps.Type.OriginalDefinition.SpecialType != SpecialType.System_Nullable_T)
            {
                return $" = {ParameterValueDefault}";
            }

            return $" = {ParameterValueNull}";
        }

        string defaultValue = ps.ExplicitDefaultValue switch
        {
            string s => $"\"{s.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"",
            char c => $"'{c}'",
            bool b => b ? "true" : "false",
            IFormattable f => f.ToString(null, CultureInfo.InvariantCulture) ?? ParameterValueDefault,
            _ => ps.ExplicitDefaultValue.ToString() ?? ParameterValueDefault
        };

        return $" = {defaultValue}";
    }

    private static ExpressionSyntax? GetDefaultValueSyntax(IParameterSymbol ps)
    {
        var fromParameter =
            ps.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as ParameterSyntax
            ?? ps.OriginalDefinition.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as ParameterSyntax;

        if (fromParameter?.Default?.Value is not null)
        {
            return fromParameter.Default.Value;
        }

        if (ps.ContainingSymbol is IMethodSymbol method)
        {
            var methodSyntax = method.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as BaseMethodDeclarationSyntax
                ?? method.OriginalDefinition.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as BaseMethodDeclarationSyntax;

            if (methodSyntax is not null)
            {
                var parameterSyntax = methodSyntax.ParameterList.Parameters.FirstOrDefault(p => p.Identifier.ValueText == ps.Name);
                return parameterSyntax?.Default?.Value;
            }
        }

        return null;
    }

    private static bool IsNonNullableReferenceTypeInNullableEnabledContext(IParameterSymbol ps)
    {
        return ps.Type.IsReferenceType && ps.NullableAnnotation == NullableAnnotation.NotAnnotated;
    }

    public static TypeEnum GetTypeEnum(this IParameterSymbol p) => p.Type.GetTypeEnum();
}