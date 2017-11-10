using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SexyHttp.HttpBodies;

namespace SexyHttp.Mocks
{
    public class MockHttpHandler : IHttpHandler
    {
        public HttpApiRequest Request { get; set; }
        public Func<HttpApiRequest, HttpApiResponse> Response { get; set; }

        public MockHttpHandler(HttpStatusCode statusCode = HttpStatusCode.OK)
            : this(x => new HttpApiResponse(statusCode, new JsonHttpBody(JValue.CreateNull()), x.Headers))
        {
        }

        public MockHttpHandler(object response, HttpStatusCode statusCode = HttpStatusCode.OK)
            : this(JToken.FromObject(response))
        {
        }

        public MockHttpHandler(JToken response, HttpStatusCode statusCode = HttpStatusCode.OK)
            : this(x => new HttpApiResponse(statusCode, new JsonHttpBody(response), x.Headers))
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
            Request = request.Clone();
            return Task.FromResult(new HttpHandlerResponse(Response(request), TimeSpan.Zero, TimeSpan.Zero));
        }
    }
}