using ProxyInterfaceGenerator;

namespace ProxyInterfaceConsumer
{
    [Proxy<Person2>()]
    public partial interface IPersonGeneric
    {
    }
}