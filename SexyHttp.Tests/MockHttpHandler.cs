using System;
using System.Net;
using System.Threading.Tasks;

namespace SexyHttp.Tests
{
    public class MockHttpHandler : IHttpHandler
    {
        public HttpApiRequest Request { get; set; }
        public Func<HttpApiRequest, HttpApiResponse> Response { get; set; }

        public MockHttpHandler() : this(x => new HttpApiResponse(HttpStatusCode.Found, x.Headers, new HttpBody {  }))
        {
        }

        public MockHttpHandler(HttpApiResponse response) : this(_ => response)
        {
        }

        public MockHttpHandler(Func<HttpApiRequest, HttpApiResponse> response)
        {
            Response = response;
        }

        public Task<HttpApiResponse> Call(HttpApiRequest request)
        {
            Request = request;
            return Task.FromResult(Response(request));
        }
    }
}