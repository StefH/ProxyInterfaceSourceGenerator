using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ProxyInterfaceSourceGenerator.Utils;

internal static class SourceCodeCleaner
{
    internal static string Clean(string code)
    {
        // Parse syntax tree
        var tree = CSharpSyntaxTree.ParseText(code);
        var root = tree.GetRoot();

        // Find all method declarations
        var methodDeclarations = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .ToArray();

        // Find all method invocations
        var methodInvocations = root.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Select(inv => inv.Expression.ToString())
            .ToArray();

        // Find Unused private static methods and remove them
        foreach (var method in methodDeclarations)
        {
            var isPrivateStatic = method.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword) && m.IsKind(SyntaxKind.StaticKeyword));
            if (isPrivateStatic && !methodInvocations.Any(call => call.Contains(method.Identifier.Text)))
            {
                code = code.Replace(method.ToFullString(), string.Empty);
            }
        }

        return code;
    }
}