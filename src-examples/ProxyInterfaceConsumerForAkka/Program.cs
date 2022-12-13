using Akka.Actor;
using ProxyInterfaceConsumerForAkka.Interfaces;

namespace ProxyInterfaceConsumerForAkka;

public class Program
{
    public static void Main()
    {
        LocalActorRefProvider p = null;
        ILocalActorRefProvider proxy = new LocalActorRefProviderProxy(p);
    }
}