using Akka.Actor;

namespace ProxyInterfaceConsumerForAkka.Interfaces
{
    [ProxyInterfaceGenerator.Proxy(typeof(LocalActorRefProvider))]
    public partial interface ILocalActorRefProvider
    {
    }
}