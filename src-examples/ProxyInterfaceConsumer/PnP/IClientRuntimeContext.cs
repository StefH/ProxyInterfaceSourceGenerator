using AutoMapper;
using Microsoft.SharePoint.Client;

namespace ProxyInterfaceConsumer.PnP
{
    [ProxyInterfaceGenerator.Proxy(typeof(Microsoft.SharePoint.Client.ClientRuntimeContext))]
    public partial interface IClientRuntimeContext
    {
       
    }
}

