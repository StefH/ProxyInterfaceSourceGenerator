namespace ProxyInterfaceSourceGenerator.Types;

internal record ProxyInterfaceGeneratorAttributeArguments(string FullyQualifiedDisplayString, string MetadataName)
{
    public bool ProxyBaseClasses { get; set; }

    public ProxyClassAccessibility Accessibility { get; set; }
    public string[] MembersToIgnore { get; set; } = Array.Empty<string>();
}