using ProxyInterfaceGenerator;

namespace ProxyInterfaceConsumer
{
    [Proxy(typeof(Address), ["Weird"])]
    public partial interface IAddress
    {
    }
}