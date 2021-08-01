using Microsoft.CodeAnalysis;

namespace ProxyInterfaceSourceGenerator.Extensions
{
    internal static class SyntaxNodeUtils
    {
        // https://stackoverflow.com/questions/20458457/getting-class-fullname-including-namespace-from-roslyn-classdeclarationsyntax
        public static bool TryGetParentSyntax<T>(this SyntaxNode? syntaxNode, out T? result) where T : SyntaxNode
        {
            result = null;

            if (syntaxNode is null)
            {
                return false;
            }

            try
            {
                syntaxNode = syntaxNode.Parent;

                if (syntaxNode is null)
                {
                    return false;
                }

                if (syntaxNode.GetType() == typeof(T))
                {
                    result = syntaxNode as T;
                    return true;
                }

                return TryGetParentSyntax(syntaxNode, out result);
            }
            catch
            {
                return false;
            }
        }
    }
}