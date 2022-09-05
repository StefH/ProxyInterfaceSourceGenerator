namespace ProxyInterfaceConsumerForPnP.Interfaces
{
    [ProxyInterfaceGenerator.Proxy(typeof(Microsoft.SharePoint.Client.Web))]
    public partial interface IWeb: ISecurableObject
    {
    }
}