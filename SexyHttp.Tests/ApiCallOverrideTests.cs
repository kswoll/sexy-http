﻿using System.Threading.Tasks;
using NUnit.Framework;
using SexyHttp.Mocks;

namespace SexyHttp.Tests
{
    [TestFixture]
    public class ApiCallOverrideTests
    {
        [Test]
        public async Task Override()
        {
            using (MockHttpServer.Json(x => x))
            {
                var client = HttpApiClient<OverrideApi>.Create("http://localhost:8844");
                var result = await client.ReflectString("foo");
                Assert.AreEqual("foobargoat", result);
            }
        }

        private abstract class OverrideApi : Api
        {
            [Post]
            public abstract Task<string> ReflectString(string s);

            public override async Task<object> Call(HttpApiEndpoint endpoint, IHttpHandler httpHandler, string baseUrl, HttpApiArguments arguments, HttpApiInstrumenter apiInstrumenter)
            {
                arguments["s"] = arguments["s"] + "bar";
                var result = await base.Call(endpoint, httpHandler, baseUrl, arguments, apiInstrumenter);
                return result + "goat";
            }
        }
    }
}