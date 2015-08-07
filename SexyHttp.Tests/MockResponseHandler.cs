using System;
using System.Threading.Tasks;

namespace SexyHttp.Tests
{
    public class MockResponseHandler<T> : HttpResponseHandler
    {
        public object Response { get; set; }

        private readonly Func<HttpApiResponse, T> responseFactory;

        public MockResponseHandler(T response = default(T)) : this(_ => response)
        {
        }

        public MockResponseHandler(Func<HttpApiResponse, T> responseFactory) 
        {
            this.responseFactory = responseFactory;
        }

        public override Task<object> HandleResponse(HttpApiResponse response) 
        {
            return Task.FromResult<object>(responseFactory(response));
        }
    }
}