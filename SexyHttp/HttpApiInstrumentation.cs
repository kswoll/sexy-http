using System;
using System.Threading.Tasks;

namespace SexyHttp
{
    public class HttpApiInstrumentation : IHttpApiInstrumentation
    {
        private readonly Func<HttpApiRequest> getRequest;
        private readonly Func<HttpApiRequest, Task<HttpHandlerResponse>> getResponse;
        private readonly Func<HttpApiRequest, HttpHandlerResponse, Task<object>> getResult;

        public HttpApiInstrumentation(
            IHttpApiInstrumentation inner,
            Func<HttpApiRequest> getRequest = null,
            Func<HttpApiRequest, Task<HttpHandlerResponse>> getResponse = null,
            Func<HttpApiRequest, HttpHandlerResponse, Task<object>> getResult = null)
        {
            this.getRequest = getRequest ?? inner.GetRequest;
            this.getResponse = getResponse ?? inner.GetResponse;
            this.getResult = getResult ?? inner.GetResult;
        }

        public HttpApiRequest GetRequest()
        {
            return getRequest();
        }

        public Task<HttpHandlerResponse> GetResponse(HttpApiRequest request)
        {
            return getResponse(request);
        }

        public Task<object> GetResult(HttpApiRequest request, HttpHandlerResponse response)
        {
            return getResult(request, response);
        }
    }
}
