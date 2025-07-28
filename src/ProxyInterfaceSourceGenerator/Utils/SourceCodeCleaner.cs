using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal static class SourceCodeCleaner
{
    internal static string Clean(string code)
    {
        var tree = CSharpSyntaxTree.ParseText(code);
        var root = tree.GetRoot();

        var methodDeclarations = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .ToArray();

        var methodInvocations = root.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .ToArray();

        foreach (var method in methodDeclarations)
        {
            bool isPrivateStatic = method.Modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword)) &&
                                   method.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword));

            if (!isPrivateStatic)
            {
                continue;
            }

            string methodName = method.Identifier.Text;

            var declaredParamTypes = method.ParameterList.Parameters
                .Select(p => p.Type?.ToString()?.Trim())
                .ToArray();

            bool isUsed = methodInvocations.Any(invocation =>
            {
                string? invokedName = null;

                if (invocation.Expression is IdentifierNameSyntax id)
                {
                    invokedName = id.Identifier.Text;
                }
                else if (invocation.Expression is MemberAccessExpressionSyntax member)
                {
                    invokedName = member.Name.Identifier.Text;
                }

                if (invokedName != methodName)
                {
                    return false;
                }

                var args = invocation.ArgumentList.Arguments;

                if (args.Count != declaredParamTypes.Length)
                {
                    return false;
                }

                for (int i = 0; i < args.Count; i++)
                {
                    var expr = args[i].Expression;
                    string? argType = null;

                    if (expr is ObjectCreationExpressionSyntax obj)
                    {
                        argType = obj.Type.ToString().Trim();
                    }
                    else if (expr is IdentifierNameSyntax idName)
                    {
                        argType = idName.Identifier.Text;
                    }
                    else if (expr is MemberAccessExpressionSyntax mem)
                    {
                        argType = mem.Name.Identifier.Text;
                    }

                    if (argType == null)
                    {
                        return false;
                    }

                    if (declaredParamTypes[i] == null || !declaredParamTypes[i]!.Contains(argType))
                    {
                        return false;
                    }
                }

                return true;
            });

            if (!isUsed)
            {
                code = code.Replace(method.ToFullString(), string.Empty);
            }
        }

        return code;
    }
}