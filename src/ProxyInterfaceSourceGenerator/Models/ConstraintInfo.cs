namespace ProxyInterfaceSourceGenerator.Models;

internal record ConstraintInfo(string Type, List<string> Items)
{
    public override string ToString()
    {
        return Items.Any() ? $" where {Type} : {string.Join(", ", Items)}" : string.Empty;
    }
}