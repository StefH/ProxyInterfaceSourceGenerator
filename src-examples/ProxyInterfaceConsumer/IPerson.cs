using ProxyInterfaceGenerator;

namespace ProxyInterfaceConsumer
{
    [Proxy(typeof(ProxyInterfaceConsumer.Person), ProxyClassAccessibility.Internal)]
    public partial interface IPerson { }
}
