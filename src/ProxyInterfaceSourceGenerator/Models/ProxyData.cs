namespace ProxyInterfaceSourceGenerator.Models;

internal record ProxyData
(
    string Namespace,
    string ShortInterfaceName,
    string FullInterfaceName,
    string FullRawTypeName,
    string ShortTypeName,
    string FullTypeName,
    List<string> Usings,
    bool ProxyBaseClasses
);