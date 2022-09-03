using Microsoft.SharePoint.Client;

namespace ProxyInterfaceConsumer.PnP
{
    [ProxyInterfaceGenerator.Proxy(typeof(SecurableObject))]
    public partial interface ISecurableObject : IClientObject
    {
        // public virtual void X();
    }
}