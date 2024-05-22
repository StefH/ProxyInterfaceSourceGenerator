using System.Net.Http;

namespace ProxyInterfaceConsumer.Http;

[ProxyInterfaceGenerator.Proxy(typeof(HttpClient), true)]
public partial interface IHttpClient : IHttpMessageInvoker { }

[ProxyInterfaceGenerator.Proxy(typeof(HttpMessageInvoker))]
public partial interface IHttpMessageInvoker { }
