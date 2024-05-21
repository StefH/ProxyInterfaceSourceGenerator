using System.Diagnostics.CodeAnalysis;
using Akka.Actor;
using ProxyInterfaceConsumerForAkka.Interfaces;

namespace ProxyInterfaceConsumerForAkka;

public class Program
{
    [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
    public static void Main()
    {
        Akka.Remote.AddressUid auid = null!;
        IAddressUid addressUidProxy = new AddressUidProxy(auid);

        LocalActorRefProvider p = null!;
        ILocalActorRefProvider proxy = new LocalActorRefProviderProxy(p);
    }
}
