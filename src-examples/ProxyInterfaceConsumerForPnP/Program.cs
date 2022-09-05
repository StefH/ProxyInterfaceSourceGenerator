using System;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using PnP.Core.Model.SharePoint;
using PnP.Framework;
using ProxyInterfaceConsumerForPnP.Implementations;
using ProxyInterfaceConsumerForPnP.Interfaces;
using IWeb = ProxyInterfaceConsumerForPnP.Interfaces.IWeb;

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

            IClientContext cp = new ClientContextProxy(clientContext);

            cp.Load<IWeb, Web>(cp.Web, w => w.Lists, w => w.Language, w => w.Author);

            await cp.ExecuteQueryRetryAsync();

            Console.WriteLine(cp.Web.Title + "," + cp.Web.Language + "," + cp.Web.Author.Email);
            foreach (var list in cp.Web.Lists)
            {
                Console.WriteLine("  list : {0}", list.Title);
            }

            foreach (var list in cp.Web.Lists)
            {
                cp._Instance.Load(list, l => l.Author.Email);
            }
            await cp.ExecuteQueryRetryAsync();

            Console.WriteLine(new string('-', 80));
            foreach (var list in cp.Web.Lists)
            {
                Console.WriteLine("  list : {0} '{1}'", list.Title, list.Author.Email);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error Message: " + ex.Message);
        }
    }
}