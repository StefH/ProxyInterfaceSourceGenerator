//----------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by https://github.com/StefH/ProxyInterfaceSourceGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//----------------------------------------------------------------------------------------

#nullable enable
using System;

namespace ProxyInterfaceSourceGeneratorTests.Source
{
    public partial interface IHttpClient
    {
        new global::System.Net.Http.HttpClient _Instance { get; }

        global::System.Net.IWebProxy DefaultProxy { get; set; }

        global::System.Net.Http.Headers.HttpRequestHeaders DefaultRequestHeaders { get; }

        global::System.Version DefaultRequestVersion { get; set; }

        global::System.Net.Http.HttpVersionPolicy DefaultVersionPolicy { get; set; }

        global::System.Uri? BaseAddress { get; set; }

        global::System.TimeSpan Timeout { get; set; }

        long MaxResponseContentBufferSize { get; set; }

        global::System.Threading.Tasks.Task<string> GetStringAsync([System.Diagnostics.CodeAnalysis.StringSyntaxAttribute("Uri")] string? requestUri);

        global::System.Threading.Tasks.Task<string> GetStringAsync(global::System.Uri? requestUri);

        global::System.Threading.Tasks.Task<string> GetStringAsync([System.Diagnostics.CodeAnalysis.StringSyntaxAttribute("Uri")] string? requestUri, global::System.Threading.CancellationToken cancellationToken);

        global::System.Threading.Tasks.Task<string> GetStringAsync(global::System.Uri? requestUri, global::System.Threading.CancellationToken cancellationToken);

        global::System.Threading.Tasks.Task<byte[]> GetByteArrayAsync([System.Diagnostics.CodeAnalysis.StringSyntaxAttribute("Uri")] string? requestUri);

        global::System.Threading.Tasks.Task<byte[]> GetByteArrayAsync(global::System.Uri? requestUri);

        global::System.Threading.Tasks.Task<byte[]> GetByteArrayAsync([System.Diagnostics.CodeAnalysis.StringSyntaxAttribute("Uri")] string? requestUri, global::System.Threading.CancellationToken cancellationToken);

        global::System.Threading.Tasks.Task<byte[]> GetByteArrayAsync(global::System.Uri? requestUri, global::System.Threading.CancellationToken cancellationToken);

        global::System.Threading.Tasks.Task<global::System.IO.Stream> GetStreamAsync([System.Diagnostics.CodeAnalysis.StringSyntaxAttribute("Uri")] string? requestUri);

        global::System.Threading.Tasks.Task<global::System.IO.Stream> GetStreamAsync([System.Diagnostics.CodeAnalysis.StringSyntaxAttribute("Uri")] string? requestUri, global::System.Threading.CancellationToken cancellationToken);

        global::System.Threading.Tasks.Task<global::System.IO.Stream> GetStreamAsync(global::System.Uri? requestUri);

        global::System.Threading.Tasks.Task<global::System.IO.Stream> GetStreamAsync(global::System.Uri? requestUri, global::System.Threading.CancellationToken cancellationToken);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> GetAsync([System.Diagnostics.CodeAnalysis.StringSyntaxAttribute("Uri")] string? requestUri);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> GetAsync(global::System.Uri? requestUri);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> GetAsync([System.Diagnostics.CodeAnalysis.StringSyntaxAttribute("Uri")] string? requestUri, global::System.Net.Http.HttpCompletionOption completionOption);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> GetAsync(global::System.Uri? requestUri, global::System.Net.Http.HttpCompletionOption completionOption);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> GetAsync([System.Diagnostics.CodeAnalysis.StringSyntaxAttribute("Uri")] string? requestUri, global::System.Threading.CancellationToken cancellationToken);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> GetAsync(global::System.Uri? requestUri, global::System.Threading.CancellationToken cancellationToken);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> GetAsync([System.Diagnostics.CodeAnalysis.StringSyntaxAttribute("Uri")] string? requestUri, global::System.Net.Http.HttpCompletionOption completionOption, global::System.Threading.CancellationToken cancellationToken);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> GetAsync(global::System.Uri? requestUri, global::System.Net.Http.HttpCompletionOption completionOption, global::System.Threading.CancellationToken cancellationToken);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> PostAsync([System.Diagnostics.CodeAnalysis.StringSyntaxAttribute("Uri")] string? requestUri, global::System.Net.Http.HttpContent? content);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> PostAsync(global::System.Uri? requestUri, global::System.Net.Http.HttpContent? content);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> PostAsync([System.Diagnostics.CodeAnalysis.StringSyntaxAttribute("Uri")] string? requestUri, global::System.Net.Http.HttpContent? content, global::System.Threading.CancellationToken cancellationToken);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> PostAsync(global::System.Uri? requestUri, global::System.Net.Http.HttpContent? content, global::System.Threading.CancellationToken cancellationToken);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> PutAsync([System.Diagnostics.CodeAnalysis.StringSyntaxAttribute("Uri")] string? requestUri, global::System.Net.Http.HttpContent? content);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> PutAsync(global::System.Uri? requestUri, global::System.Net.Http.HttpContent? content);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> PutAsync([System.Diagnostics.CodeAnalysis.StringSyntaxAttribute("Uri")] string? requestUri, global::System.Net.Http.HttpContent? content, global::System.Threading.CancellationToken cancellationToken);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> PutAsync(global::System.Uri? requestUri, global::System.Net.Http.HttpContent? content, global::System.Threading.CancellationToken cancellationToken);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> PatchAsync([System.Diagnostics.CodeAnalysis.StringSyntaxAttribute("Uri")] string? requestUri, global::System.Net.Http.HttpContent? content);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> PatchAsync(global::System.Uri? requestUri, global::System.Net.Http.HttpContent? content);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> PatchAsync([System.Diagnostics.CodeAnalysis.StringSyntaxAttribute("Uri")] string? requestUri, global::System.Net.Http.HttpContent? content, global::System.Threading.CancellationToken cancellationToken);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> PatchAsync(global::System.Uri? requestUri, global::System.Net.Http.HttpContent? content, global::System.Threading.CancellationToken cancellationToken);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> DeleteAsync([System.Diagnostics.CodeAnalysis.StringSyntaxAttribute("Uri")] string? requestUri);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> DeleteAsync(global::System.Uri? requestUri);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> DeleteAsync([System.Diagnostics.CodeAnalysis.StringSyntaxAttribute("Uri")] string? requestUri, global::System.Threading.CancellationToken cancellationToken);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> DeleteAsync(global::System.Uri? requestUri, global::System.Threading.CancellationToken cancellationToken);

        [System.Runtime.Versioning.UnsupportedOSPlatformAttribute("browser")]
        global::System.Net.Http.HttpResponseMessage Send(global::System.Net.Http.HttpRequestMessage request);

        [System.Runtime.Versioning.UnsupportedOSPlatformAttribute("browser")]
        global::System.Net.Http.HttpResponseMessage Send(global::System.Net.Http.HttpRequestMessage request, global::System.Net.Http.HttpCompletionOption completionOption);

        [System.Runtime.Versioning.UnsupportedOSPlatformAttribute("browser")]
        global::System.Net.Http.HttpResponseMessage Send(global::System.Net.Http.HttpRequestMessage request, global::System.Threading.CancellationToken cancellationToken);

        [System.Runtime.Versioning.UnsupportedOSPlatformAttribute("browser")]
        global::System.Net.Http.HttpResponseMessage Send(global::System.Net.Http.HttpRequestMessage request, global::System.Net.Http.HttpCompletionOption completionOption, global::System.Threading.CancellationToken cancellationToken);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> SendAsync(global::System.Net.Http.HttpRequestMessage request);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> SendAsync(global::System.Net.Http.HttpRequestMessage request, global::System.Threading.CancellationToken cancellationToken);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> SendAsync(global::System.Net.Http.HttpRequestMessage request, global::System.Net.Http.HttpCompletionOption completionOption);

        global::System.Threading.Tasks.Task<global::System.Net.Http.HttpResponseMessage> SendAsync(global::System.Net.Http.HttpRequestMessage request, global::System.Net.Http.HttpCompletionOption completionOption, global::System.Threading.CancellationToken cancellationToken);

        void CancelPendingRequests();
    }
}
#nullable restore