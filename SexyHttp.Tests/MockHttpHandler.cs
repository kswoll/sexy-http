using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SexyHttp.HttpBodies;

namespace SexyHttp.Tests
{
    public class MockHttpHandler : IHttpHandler
    {
        public HttpApiRequest Request { get; set; }
        public Func<HttpApiRequest, HttpApiResponse> Response { get; set; }

        public MockHttpHandler() : this(x => new HttpApiResponse(HttpStatusCode.OK, new JsonHttpBody(JValue.CreateNull()), x.Headers))
        {
        }

        public MockHttpHandler(HttpApiResponse response) : this(_ => response)
        {
        }

        public MockHttpHandler(Func<HttpApiRequest, HttpApiResponse> response)
        {
            Response = response;
        }

        public Task<HttpHandlerResponse> Call(HttpApiRequest request)
        {
            Request = request;
            return Task.FromResult(new HttpHandlerResponse(Response(request), TimeSpan.Zero, TimeSpan.Zero));
        }
    }
}