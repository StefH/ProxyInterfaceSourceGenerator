using ProxyInterfaceGenerator;

namespace Stef
{
    [Proxy<Class1>)]
    public partial interface IClass1
    {
    }

    [Proxy<Class2>)]
    public partial interface IClass2
    {
    }
}

namespace Microsoft.Graph.Admin
{
    [Proxy(typeof(AdminRequestBuilder), true)]
    public partial interface IAdminRequestBuilder;
}

namespace Microsoft.Graph.Admin.Edge
{
    [Proxy(typeof(EdgeRequestBuilder), true)]
    public partial interface IEdgeRequestBuilder;
}

namespace Microsoft.Graph
{
    [Proxy<GraphServiceClient>(true)]
    public partial interface IGraphServiceClient;
}

namespace Microsoft.Graph
{
    [Proxy<BatchRequestBuilder>(true)]
    public partial interface IBatchRequestBuilder;
}