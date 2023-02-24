namespace ProxyInterfaceSourceGenerator.Types;

internal record ProxyInterfaceGeneratorAttributeArguments(string RawTypeName)
{
    public bool ProxyBaseClasses { get; set; }

    public ProxyInterfaceGeneratorAccessibility Accessibility { get; set; }
}