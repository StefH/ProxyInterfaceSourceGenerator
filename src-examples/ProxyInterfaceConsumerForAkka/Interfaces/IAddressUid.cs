using Akka.Remote;

// namespace ProxyInterfaceConsumerForAkka.Interfaces;     <-- no namespace

[ProxyInterfaceGenerator.Proxy(typeof(AddressUid))]
// ReSharper disable once CheckNamespace
public partial interface IAddressUid
{
}