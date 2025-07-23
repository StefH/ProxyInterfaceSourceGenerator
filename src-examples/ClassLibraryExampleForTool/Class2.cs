using Microsoft.Graph.Admin;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace Stef;

public class Class2
{
    public Class2()
    {
        var builder = new AdminRequestBuilder("", new HttpClientRequestAdapter(new AnonymousAuthenticationProvider()));
        builder.GetAsync();
    }
}