namespace ProxyInterfaceConsumer.PnP
{
    [ProxyInterfaceGenerator.Proxy(typeof(Microsoft.SharePoint.Client.Web))]
    public partial interface IWeb: ISecurableObject
    {
    }
}