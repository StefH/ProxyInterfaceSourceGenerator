using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using ProxyInterfaceConsumerForPnP.Interfaces;

namespace ProxyInterfaceConsumerForPnP.Implementations;

public static class ClientRuntimeContextExtensions
{
    public static Task ExecuteQueryRetryAsync(this IClientRuntimeContext clientContext, int retryCount = 10, string? userAgent = null)
    {
        ClientRuntimeContext clientObject_ = Mapster.TypeAdapter.Adapt<ClientRuntimeContext>(clientContext);
        return clientObject_.ExecuteQueryRetryAsync(retryCount, userAgent);
    }
}