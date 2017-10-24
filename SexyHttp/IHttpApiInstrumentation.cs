using System.Collections.Generic;
using System.Threading.Tasks;

namespace SexyHttp
{
    public interface IHttpApiInstrumentation
    {
        /// <summary>
        /// Produces an instance of HttpApiRequest based on the values of the arguments array of the original instrumentation.
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

        object InterleaveResult(HttpApiRequest request, HttpHandlerResponse response, object lastResult, object result);
    }
}
