using System;
using Microsoft.Graph.Core.Requests;

namespace Microsoft.Graph
{
    public partial class GraphServiceClientProxy : Graph.IGraphServiceClient
    {
        BatchRequestBuilder IBaseClient.Batch => throw new NotImplementedException();
    }
}