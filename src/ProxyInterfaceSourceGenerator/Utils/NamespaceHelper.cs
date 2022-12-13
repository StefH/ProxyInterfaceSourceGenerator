namespace ProxyInterfaceSourceGenerator.Utils;

internal static class NamespaceBuilder
{
    public static (string Start, string End) Build(string ns)
    {
        var namespaceDefined = !string.IsNullOrEmpty(ns);

        return
        (
            namespaceDefined ? $"namespace {ns}\r\n{{" : string.Empty,
            namespaceDefined ? "}" : string.Empty
        );
    }
}