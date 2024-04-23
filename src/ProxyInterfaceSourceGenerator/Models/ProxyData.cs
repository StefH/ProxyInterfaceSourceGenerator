using ProxyInterfaceSourceGenerator.Types;

namespace ProxyInterfaceSourceGenerator.Models;

internal class ProxyData
{
    public ProxyData(string @namespace,
                     string namespaceDot,
                     string shortInterfaceName,
                     string fullInterfaceName,
                     string fullRawTypeName,
                     string shortTypeName,
                     string fullTypeName,
                     List<string> usings,
                     bool proxyBaseClasses,
                     ProxyClassAccessibility accessibility)
    {
        Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
        NamespaceDot = namespaceDot ?? throw new ArgumentNullException(nameof(namespaceDot));
        ShortInterfaceName = shortInterfaceName ?? throw new ArgumentNullException(nameof(shortInterfaceName));
        FullInterfaceName = fullInterfaceName ?? throw new ArgumentNullException(nameof(fullInterfaceName));
        FullRawTypeName = fullRawTypeName ?? throw new ArgumentNullException(nameof(fullRawTypeName));
        ShortTypeName = shortTypeName ?? throw new ArgumentNullException(nameof(shortTypeName));
        FullTypeName = fullTypeName ?? throw new ArgumentNullException(nameof(fullTypeName));
        Usings = usings ?? throw new ArgumentNullException(nameof(usings));
        ProxyBaseClasses = proxyBaseClasses;
        Accessibility = accessibility;
    }

    public string Namespace { get; }

    public string NamespaceDot { get; }

    public string ShortInterfaceName { get; }

    public string FullInterfaceName { get; }

    public string FullRawTypeName { get; }

    public string ShortTypeName { get; }

    public string FullTypeName { get; }

    public List<string> Usings { get; }

    public bool ProxyBaseClasses { get; }

    public ProxyClassAccessibility Accessibility { get; }
}