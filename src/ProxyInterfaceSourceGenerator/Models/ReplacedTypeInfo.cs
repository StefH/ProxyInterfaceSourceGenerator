using ProxyInterfaceSourceGenerator.Types;

namespace ProxyInterfaceSourceGenerator.Models;

internal class ReplacedTypeInfo
{
    public string ClassType { get; }

    public string InterfaceType { get; }

    public string ElementType { get; }

    public string ElementInterfaceType { get; }

    public string Proxy { get; }

    public bool Direct { get; }

    public TypeUsedIn UsedIn { get; set; }

    internal ReplacedTypeInfo(
        string classType,
        string interfaceType,
        string elementType,
        string elementInterfaceType,
        string proxy,
        bool direct,
        TypeUsedIn usedIn)
    {
        ClassType = classType;
        InterfaceType = interfaceType;
        ElementType = elementType;
        ElementInterfaceType = elementInterfaceType;
        Proxy = proxy;
        Direct = direct;
        UsedIn = usedIn;
    }
}