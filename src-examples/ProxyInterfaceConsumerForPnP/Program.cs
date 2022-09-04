using Microsoft.SharePoint.Client;
using ProxyInterfaceConsumer.PnP;

namespace ProxyInterfaceConsumerForPnP
{
    public class Program
    {
        public static void Main()
        {
            var cp = new ClientContextProxy(new ClientContext("https://heyenrath.nl"));
            cp.Test();
        }
    }
}