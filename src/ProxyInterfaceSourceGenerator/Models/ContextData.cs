namespace ProxyInterfaceSourceGenerator.Models;

internal record ContextData
{
    public string? InterfaceName { get; init; }

    public string? ClassName { get; init; }

    public FileData FileData { get; init; } = default!;
}