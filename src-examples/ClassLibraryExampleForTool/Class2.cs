using Microsoft.Graph;
using Microsoft.Graph.Admin;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace Stef;

public class Class2
{
    public Class2()
    {
        var c = new GraphServiceClient((IRequestAdapter)null!, "");
        c.Batch
        var builder = new AdminRequestBuilder("", new HttpClientRequestAdapter(new AnonymousAuthenticationProvider()));
        builder.GetAsync();
    }
}