namespace ProxyInterfaceConsumer.Http
{
    [ProxyInterfaceGenerator.Proxy(typeof(System.Net.Http.HttpClient, true))]
    public partial interface IHttpClient : IHttpMessageInvoker
    {
        
    }

    [ProxyInterfaceGenerator.Proxy(typeof(System.Net.Http.HttpMessageInvoker))]
    public partial interface IHttpMessageInvoker
    {

    }
}