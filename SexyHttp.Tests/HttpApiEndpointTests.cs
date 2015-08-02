using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SexyHttp.Tests
{
    [TestFixture]
    public class HttpApiEndpointTests
    {
        [Test]
        public async void HeadersProvider()
        {
            var httpHandler = new MockHttpHandler();
            var responseHandler = new MockResponseHandler();

            var headersProvider = new MockHeadersProvider();
            headersProvider.Headers.Add(new HttpHeader("key", "value"));

            var endpoint = new HttpApiEndpoint("path/to/api", httpHandler, headersProvider, new Dictionary<string, IHttpArgumentHandler>(), responseHandler);
            await endpoint.Call("http://localhost", new Dictionary<string, object>());

            var header = httpHandler.Request.Headers.Single();
            Assert.AreEqual("key", header.Name);
            Assert.AreEqual("value", header.Values.Single());
        }

        [Test]
        public async void ArgumentProvider()
        {
            var httpHandler = new MockHttpHandler();
            var responseHandler = new MockResponseHandler(x => x.Headers.Single(y => y.Name == "name").Values.Single());

            var endpoint = new HttpApiEndpoint(
                "path/to/api", 
                httpHandler, 
                new MockHeadersProvider(), 
                new Dictionary<string, IHttpArgumentHandler>
                {
                    { "name", new HttpHeaderArgumentHandler("value")  }
                }, 
                responseHandler);
            var response = await endpoint.Call("http://localhost", new Dictionary<string, object> { { "name", "value" } });

            Assert.AreEqual("value", response);
        }
    }
}