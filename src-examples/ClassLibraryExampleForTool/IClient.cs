namespace Microsoft.Graph
{
    [Proxy<GraphServiceClient>(true)]
    public partial interface IGraphServiceClient;
}

namespace Microsoft.Graph.Core.Requests
{
    [Proxy<BatchRequestBuilder>(true)]
    public partial interface IBatchRequestBuilder;
}