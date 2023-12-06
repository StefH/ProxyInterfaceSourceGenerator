// ReSharper disable once CheckNamespace
namespace System.Net.Http;

[ProxyInterfaceGenerator.Proxy(typeof(HttpClient), true)]
public partial interface IHttpClient : IHttpMessageInvoker
{
}