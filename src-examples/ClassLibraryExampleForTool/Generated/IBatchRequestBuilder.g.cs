using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;

namespace Microsoft.Graph.Core.Requests
{
    public partial interface IBatchRequestBuilder
    {
        BatchRequestBuilder _Instance { get; }

        Task<BatchResponseContent> PostAsync(BatchRequestContent batchRequestContent, CancellationToken cancellationToken = default(CancellationToken), Dictionary<string, ParsableFactory<IParsable>> errorMappings = null);

        Task<BatchResponseContentCollection> PostAsync(BatchRequestContentCollection batchRequestContentCollection, CancellationToken cancellationToken = default(CancellationToken), Dictionary<string, ParsableFactory<IParsable>> errorMappings = null);

        Task<RequestInformation> ToPostRequestInformationAsync(BatchRequestContent batchRequestContent, CancellationToken cancellationToken = default(CancellationToken));
    }
}