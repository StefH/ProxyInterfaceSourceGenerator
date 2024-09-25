using ProxyInterfaceSourceGenerator.Types;

namespace ProxyInterfaceSourceGenerator.Models;

internal class ProxyData
{
    public string Namespace { get; }

    public string NamespaceDot { get; }

    public string ShortInterfaceName { get; }

    public string FullInterfaceName { get; }

    public string FullQualifiedTypeName { get; }

    public string ShortMetadataName { get; }

    public string FullMetadataTypeName { get; }

    public List<string> Usings { get; }

    public bool ProxyBaseClasses { get; }

    public string[] MembersToIgnore { get; }

    public ProxyClassAccessibility Accessibility { get; }

    public ProxyData(
        string @namespace,
        string namespaceDot,
        string shortInterfaceName,
        string fullInterfaceName,
        string fullQualifiedTypeName,
        string shortMetadataTypeName,
        string fullMetadataTypeName,
        List<string> usings,
        bool proxyBaseClasses,
        ProxyClassAccessibility accessibility,
        string[] membersToIgnore)
    {
        Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
        NamespaceDot = namespaceDot ?? throw new ArgumentNullException(nameof(namespaceDot));
        ShortInterfaceName = shortInterfaceName ?? throw new ArgumentNullException(nameof(shortInterfaceName));
        FullInterfaceName = fullInterfaceName ?? throw new ArgumentNullException(nameof(fullInterfaceName));
        FullQualifiedTypeName = fullQualifiedTypeName ?? throw new ArgumentNullException(nameof(fullQualifiedTypeName));
        ShortMetadataName = shortMetadataTypeName ?? throw new ArgumentNullException(nameof(shortMetadataTypeName));
        FullMetadataTypeName = fullMetadataTypeName ?? throw new ArgumentNullException(nameof(fullMetadataTypeName));
        Usings = usings ?? throw new ArgumentNullException(nameof(usings));
        ProxyBaseClasses = proxyBaseClasses;
        Accessibility = accessibility;
        MembersToIgnore = membersToIgnore ?? throw new ArgumentNullException(nameof(membersToIgnore));
    }
}