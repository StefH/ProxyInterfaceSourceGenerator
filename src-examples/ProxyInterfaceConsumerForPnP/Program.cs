using System;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using PnP.Framework;
using ProxyInterfaceConsumer.PnP;

namespace ProxyInterfaceConsumerForPnP;

public class Program
{
    public static async Task Main()
    {
        try
        {
            var authManager = new AuthenticationManager(
                "15b347bf-90a2-4c16-aa76-5a3263476b59",
                "Test.pfx",
                Environment.GetEnvironmentVariable("Test.pfx_PWD"),
                "s7gb6.onmicrosoft.com");

            using var clientContext = await authManager.GetContextAsync("https://s7gb6.sharepoint.com/sites/Test");
            clientContext.Load(clientContext.Web, p => p.Title);
            await clientContext.ExecuteQueryRetryAsync();

            Console.WriteLine(clientContext.Web.Title);

            var cp = new ClientContextProxy(clientContext);

            cp.Load<IWeb, Web>(cp.Web, w => w.Lists, w => w.Language);

            await cp.ExecuteQueryAsync();

            Console.WriteLine(cp.Web.Title + ": " + cp.Web.Language);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error Message: " + ex.Message);
        }
    }
}