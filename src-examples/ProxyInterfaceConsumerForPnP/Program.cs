using Microsoft.SharePoint.Client;
using ProxyInterfaceConsumer.PnP;

namespace ProxyInterfaceConsumerForPnP;

public class Program
{
    public static void Main()
    {
        var cp = new ClientContextProxy(new ClientContext("https://heyenrath.nl"));
        //cp.Test();

        cp.Load3(cp.Web, w => w.Lists, w => w.Language);

        cp.Load4<IWeb, Web>(cp.Web, w => w.Lists, w => w.Language);

        cp.ExecuteQuery();
    }
}