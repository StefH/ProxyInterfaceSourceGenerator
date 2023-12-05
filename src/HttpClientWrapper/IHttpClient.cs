using System.Net.Http;

namespace HttpClientWrapper;

[ProxyInterfaceGenerator.Proxy(typeof(HttpClient), true)]
public partial interface IHttpClient : IHttpMessageInvoker
{
}