﻿using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SexyHttp.HttpHandlers;
using SexyProxy;

namespace SexyHttp.Tests
{
    [TestFixture]
    public class HttpWebClientHttpHandlerTests
    {
        [Test]
        public void GetString()
        {
            using (MockHttpServer.ReturnJson(request => Task.FromResult<JToken>(new JValue("foo"))))
            {
                var client = HttpApiClient<EmitBasedClientTests.IGetString>.Create("http://localhost:8844/path", httpHandler: new WebClientHttpHandler());
                client.GetString().Wait();
            }
        }

        [Test]
        public void ReflectValue()
        {
            using (MockHttpServer.Json(x => x))
            {
                var client = HttpApiClient<IReflectValue>.Create("http://localhost:8844/path");
                var result = client.ReflectValue("foo").Result;
                Assert.AreEqual("foo", result);
            }
        }

        [Proxy]
        private interface IReflectValue
        {
            [Post("path")]
            Task<string> ReflectValue(string argument);
        }
    }
}