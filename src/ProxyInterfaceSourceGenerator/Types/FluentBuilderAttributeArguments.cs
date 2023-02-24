namespace ProxyInterfaceSourceGenerator.Types;

internal record ProxyInterfaceGeneratorAttributeArguments
{
    public string RawTypeName { get; set; } = null!;

    public bool ProxyBaseClasses { get; set; }

    public ProxyInterfaceGeneratorAccessibility Accessibility { get; set; }
}