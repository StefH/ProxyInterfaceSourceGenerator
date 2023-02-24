namespace ProxyInterfaceSourceGenerator.Types;

internal record ProxyInterfaceGeneratorAttributeArguments(string RawTypeName)
{
    public bool ProxyBaseClasses { get; set; }

    public ProxyClassAccessibility Accessibility { get; set; }
}