using System;

namespace SexyHttp.Tests
{
    public class MockResponseHandler : IHttpResponseHandler
    {
        public object Response { get; set; }

        private readonly Func<HttpApiResponse, object> responseFactory;

        public MockResponseHandler() : this(_ => null)
        {
        }

        public MockResponseHandler(object response = null) : this(_ => response)
        {
        }

        public MockResponseHandler(Func<HttpApiResponse, object> responseFactory)
        {
            this.responseFactory = responseFactory;
        }

        public object HandleResponse(HttpApiResponse response) 
        {
            return responseFactory(response);
        }
    }
}