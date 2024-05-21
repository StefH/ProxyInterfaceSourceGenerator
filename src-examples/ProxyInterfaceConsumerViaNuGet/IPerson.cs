using ProxyInterfaceGenerator;

namespace ProxyInterfaceConsumer
{
    [Proxy(typeof(Person), ProxyClassAccessibility.Internal)]
    public partial interface IPerson { }
}
