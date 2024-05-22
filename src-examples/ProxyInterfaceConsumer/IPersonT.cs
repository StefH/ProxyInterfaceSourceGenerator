namespace ProxyInterfaceConsumer
{
    [ProxyInterfaceGenerator.Proxy(typeof(ProxyInterfaceConsumer.PersonT<>))]
    public partial interface IPersonT //<T> where T : struct
    { }
}
