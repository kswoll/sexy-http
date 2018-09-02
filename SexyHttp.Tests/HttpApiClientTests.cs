using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using SexyHttp.Mocks;
using SexyProxy;

namespace SexyHttp.Tests
{
    [TestFixture]
    public class HttpApiClientTests
    {
        [Test]
        public async Task GetString()
        {
            var httpHandler = new MockHttpHandler(x => new HttpApiResponse(body: x.Body));
            var client = HttpApiClient<IApi>.Create("http://localhost", httpHandler: httpHandler);
            var result = await client.GetString("foo");
            Assert.AreEqual("foo", result);
        }

        [Test]
        public async Task ExceptionPropagated()
        {
            var httpHandler = new MockHttpHandler(x => new HttpApiResponse(body: x.Body));
            var client = HttpApiClient<IApi>.Create("http://localhost", httpHandler: httpHandler, apiInstrumenter: (endpoint, arguments, inner) => new ExceptionOnGetResponseInstrumentation(inner));
            try
            {
                await client.GetString("foo");
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Fail", ex.Message);
            }
        }

        [Test]
        public async Task ExceptionPropagatedAsync()
        {
            var httpHandler = new MockHttpHandler(x => new HttpApiResponse(body: x.Body));
            var client = HttpApiClient<IApi>.Create("http://localhost", httpHandler: httpHandler, apiInstrumenter: (endpoint, arguments, inner) => new ExceptionOnGetResponseInstrumentationAsync(inner));
            try
            {
                await client.GetString("foo");
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Fail", ex.Message);
            }
        }

        class ExceptionOnGetResponseInstrumentation : HttpApiInstrumentation
        {
            public ExceptionOnGetResponseInstrumentation(IHttpApiInstrumentation inner, Func<IEnumerable<HttpApiRequest>> getRequests = null, Func<HttpApiRequest, Task<HttpHandlerResponse>> getResponse = null, Func<HttpApiRequest, HttpHandlerResponse, Task<object>> getResult = null) : base(inner, getRequests, getResponse, getResult)
            {
            }

            public override Task<HttpHandlerResponse> GetResponse(HttpApiRequest request)
            {
                throw new Exception("Fail");
            }
        }

        class ExceptionOnGetResponseInstrumentationAsync : HttpApiInstrumentation
        {
            public ExceptionOnGetResponseInstrumentationAsync(IHttpApiInstrumentation inner, Func<IEnumerable<HttpApiRequest>> getRequests = null, Func<HttpApiRequest, Task<HttpHandlerResponse>> getResponse = null, Func<HttpApiRequest, HttpHandlerResponse, Task<object>> getResult = null) : base(inner, getRequests, getResponse, getResult)
            {
            }

            public override async Task<HttpHandlerResponse> GetResponse(HttpApiRequest request)
            {
                await Task.Delay(1);
                throw new Exception("Fail");
            }
        }

        [Proxy]
        public interface IApi
        {
            [Get("path")]
            Task<string> GetString(string argument);
        }

        [Test]
        public async Task InterfaceHandlerForPropertyGet()
        {
            var httpHandler = new MockHttpHandler(x => new HttpApiResponse(body: x.Body));
            var accessToken = "foo";
            var client = HttpApiClient<IInterfaceHandlerApi>.Create("http://localhost", httpHandler: httpHandler, interfaceHandler: invocation =>
            {
                switch (invocation.Method.Name)
                {
                    case "get_AccessToken":
                        return Task.FromResult((object)accessToken);
                }
                return invocation.Proceed();
            });
            Assert.AreEqual(accessToken, client.AccessToken);

            var result = await client.GetString("bar");
            Assert.AreEqual("bar", result);
        }

        [Proxy]
        public interface IInterfaceHandlerApi
        {
            string AccessToken { get; }

            [Get("path")]
            Task<string> GetString(string argument);
        }
    }
}