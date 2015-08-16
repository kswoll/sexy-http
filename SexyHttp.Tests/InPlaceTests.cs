﻿using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SexyHttp.HttpBodies;
using SexyHttp.Utils;
using SexyProxy;

namespace SexyHttp.Tests
{
    [TestFixture]
    public class InPlaceTests
    {
        [Test]
        public async void GetStringAbstract()
        {
            using (MockHttpServer.ReturnJson(request => Task.FromResult<JToken>(new JValue("foo"))))
            {
                var client = HttpApiClient<GetStringApi>.Create("http://localhost:8844/path");
                var result = await client.GetString();
                Assert.AreEqual("foo", result);
            }
        }

        private abstract class GetStringApi : Api
        {
            [Get("path")]
            public abstract Task<string> GetString();
        }

        [Test]
        public async void CustomizeInvocation()
        {
            using (MockHttpServer.ReturnJson(request => Task.FromResult<JToken>(new JValue("foo"))))
            {
                var client = HttpApiClient<CustomizeInvocationApi>.Create("http://localhost:8844/path");
                var result = await client.PostString("foo");
                Assert.AreEqual("foofoo", result);
            }
        }

        private abstract class CustomizeInvocationApi : Api
        {
            [Post("path")]
            public async Task<string> PostString(string input)
            {
                var result = (string)await this.Invocation().Proceed();
                return result + result;
            }
        }

        [Test]
        public async void InstrumentedRequest()
        {
            using (MockHttpServer.ReturnJson(request => request.Headers.Get("Test")))
            {
                var client = HttpApiClient<InstrumentRequestClass>.Create("http://localhost:8844");
                var result = await client.InstrumentedRequest();
                Assert.AreEqual("Value", result);
            }            
        }

        [Test]
        public async void InstrumentedResponse()
        {
            using (MockHttpServer.ReturnJson(request => request.Headers.Get("Test")))
            {
                var client = HttpApiClient<InstrumentResponseClass>.Create("http://localhost:8844");
                var result = await client.InstrumentedResponse();
                Assert.AreEqual("foo", result);
            }            
        }

        private abstract class InstrumentRequestClass : Api, IHttpApiInstrumenter
        {
            [Get]
            public abstract Task<string> InstrumentedRequest();

            public Task InstrumentRequest(HttpApiRequest request)
            {
                request.Headers.Add(new HttpHeader("Test", "Value"));
                return TaskConstants.Completed;
            }

            public Task InstrumentResponse(HttpApiResponse response)
            {
                return TaskConstants.Completed;
            }
        }

        private abstract class InstrumentResponseClass : Api, IHttpApiInstrumenter
        {
            [Get]
            public abstract Task<string> InstrumentedResponse();

            public Task InstrumentRequest(HttpApiRequest request)
            {
                return TaskConstants.Completed;
            }

            public Task InstrumentResponse(HttpApiResponse response)
            {
                response.Body = new JsonHttpBody("foo");
                return TaskConstants.Completed;
            }
        }
    }
}