using System.Net.Http;

namespace HttpClientWrapper;

[ProxyInterfaceGenerator.Proxy(typeof(HttpMessageInvoker))]
public partial interface IHttpMessageInvoker
{
}