using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SexyHttp
{
    public class HttpApiInstrumentation : IHttpApiInstrumentation
    {
        private readonly Func<IEnumerable<HttpApiRequest>> getRequests;
        private readonly Func<HttpApiRequest, Task<HttpHandlerResponse>> getResponse;
        private readonly Func<HttpApiRequest, HttpHandlerResponse, Task<object>> getResult;

        public HttpApiInstrumentation(
            IHttpApiInstrumentation inner,
            Func<IEnumerable<HttpApiRequest>> getRequests = null,
            Func<HttpApiRequest, Task<HttpHandlerResponse>> getResponse = null,
            Func<HttpApiRequest, HttpHandlerResponse, Task<object>> getResult = null)
        {
            this.getRequests = getRequests ?? inner.GetRequests;
            this.getResponse = getResponse ?? inner.GetResponse;
            this.getResult = getResult ?? inner.GetResult;
        }

        public virtual IEnumerable<HttpApiRequest> GetRequests()
        {
            return getRequests();
        }

        public virtual Task<HttpHandlerResponse> GetResponse(HttpApiRequest request)
        {
            return getResponse(request);
        }

        public virtual Task<object> GetResult(HttpApiRequest request, HttpHandlerResponse response)
        {
            return getResult(request, response);
        }

        public virtual object AggregateResult(HttpApiRequest request, HttpHandlerResponse response, object lastResult, object result)
        {
            return result;
        }
    }
}
