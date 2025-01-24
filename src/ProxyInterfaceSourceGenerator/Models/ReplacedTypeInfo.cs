namespace ProxyInterfaceSourceGenerator.Models;

internal record ReplacedTypeInfo
(
    string ClassType,
    string InterfaceType,
    string ElementType,
    string ElementInterfaceType,
    string Proxy,
    bool Direct
);