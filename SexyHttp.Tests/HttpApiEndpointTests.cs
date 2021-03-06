﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using SexyHttp.ArgumentHandlers;
using SexyHttp.Mocks;
using SexyHttp.TypeConverters;

namespace SexyHttp.Tests
{
    [TestFixture]
    public class HttpApiEndpointTests
    {
        [Test]
        public async Task Url()
        {
            var httpHandler = new MockHttpHandler();
            var responseHandler = new MockResponseHandler<object>();

            var endpoint = new HttpApiEndpoint(null, "path/to/api", HttpMethod.Get, new Dictionary<string, IHttpArgumentHandler>(), responseHandler, Enumerable.Empty<HttpHeader>());
            await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object>());

            Assert.AreEqual("http://localhost/path/to/api", httpHandler.Request.Url.ToString());
        }

        [Test]
        public async Task Method()
        {
            var httpHandler = new MockHttpHandler();
            var responseHandler = new MockResponseHandler<object>();

            var endpoint = new HttpApiEndpoint(null, "path/to/api", HttpMethod.Get, new Dictionary<string, IHttpArgumentHandler>(), responseHandler, Enumerable.Empty<HttpHeader>());
            await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object>());

            Assert.AreEqual(HttpMethod.Get, httpHandler.Request.Method);
        }

        [Test]
        public async Task HeadersProvider()
        {
            var httpHandler = new MockHttpHandler();
            var responseHandler = new MockResponseHandler<object>();

            var headersProvider = new MockApiInstrumenter();
            headersProvider.Headers.Add(new HttpHeader("key", "value"));

            var endpoint = new HttpApiEndpoint(null, "path/to/api", HttpMethod.Get, new Dictionary<string, IHttpArgumentHandler>(), responseHandler, Enumerable.Empty<HttpHeader>());
            await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object>(), headersProvider.InstrumentCall);

            var header = httpHandler.Request.Headers.Single();
            Assert.AreEqual("key", header.Name);
            Assert.AreEqual("value", header.Values.Single());
        }

        [Test]
        public async Task ArgumentProvider()
        {
            var httpHandler = new MockHttpHandler();
            var responseHandler = new MockResponseHandler<string>(x => x.Headers.Single(y => y.Name == "name").Values.Single());

            var endpoint = new HttpApiEndpoint(
                null,
                "path/to/api",
                HttpMethod.Get,
                new Dictionary<string, IHttpArgumentHandler>
                {
                    { "name", new HttpHeaderArgumentHandler(DefaultTypeConverter.Create())  }
                },
                responseHandler,
                Enumerable.Empty<HttpHeader>());
            var response = await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object> { ["name"] = "value" });

            Assert.AreEqual("value", response);
        }

        [Test]
        public async Task ResponseHandler()
        {
            var httpHandler = new MockHttpHandler();
            var responseHandler = new MockResponseHandler<string>("foo");

            var endpoint = new HttpApiEndpoint(
                null,
                "path/to/api",
                HttpMethod.Get,
                new Dictionary<string, IHttpArgumentHandler>(),
                responseHandler,
                Enumerable.Empty<HttpHeader>());
            var response = await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object>());

            Assert.AreEqual("foo", response);
        }
    }
}