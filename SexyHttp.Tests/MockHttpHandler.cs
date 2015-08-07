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

        public MockHttpHandler() : this(x => new HttpApiResponse(HttpStatusCode.Found, new JsonHttpBody(JValue.CreateNull()), x.Headers))
        {
        }

        public MockHttpHandler(HttpApiResponse response) : this(_ => response)
        {
        }

        public MockHttpHandler(Func<HttpApiRequest, HttpApiResponse> response)
        {
            Response = response;
        }

        public Task<T> Call<T>(HttpApiRequest request, Func<HttpApiResponse, Task<T>> responseHandler)
        {
            Request = request;
            return responseHandler(Response(request));
        }
    }
}