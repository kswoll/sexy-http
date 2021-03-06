﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace SexyHttp
{
    public interface IHttpApiInstrumentation
    {
        /// <summary>
        /// Produces an instance of HttpApiRequest based on the values of the arguments array of the
        /// original call. Usually this will return one request, but you can choose to return multiple
        /// requests for a single call.  In that case, multiple separate calls will be made, and you
        /// can put them back together into a single result via the AggregateResult method.
        /// </summary>
        IEnumerable<HttpApiRequest> GetRequests();

        /// <summary>
        /// Produces a response based on the (possibly instrumented) request.
        /// </summary>
        Task<HttpHandlerResponse> GetResponse(HttpApiRequest request);

        /// <summary>
        /// Produces the final return value for the original method of the API.
        /// </summary>
        Task<object> GetResult(HttpApiRequest request, HttpHandlerResponse response);

        /// <summary>
        /// Useful when GetRequests() returns more than one request, so that you can aggregate
        /// multiple calls to an API and combine them back into a single result.
        /// </summary>
        object AggregateResult(HttpApiRequest request, HttpHandlerResponse response, object lastResult, object result);
    }
}
