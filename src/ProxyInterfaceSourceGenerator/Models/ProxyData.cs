namespace ProxyInterfaceSourceGenerator.Models;

internal record ProxyData
(
    string Namespace,
    string ShortInterfaceName,
    string FullInterfaceName,
    string RawTypeName,
    string TypeName,
    List<string> Usings,
    bool ProxyBaseClasses
);