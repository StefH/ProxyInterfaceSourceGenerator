using ProxyInterfaceGenerator;

namespace ProxyInterfaceConsumer
{
    [ProxyInterfaceGenerator.Proxy(typeof(Address), false, ProxyClassAccessibility.Public, new []{"Weird"})]
    public partial interface IAddress
    {
    }
}