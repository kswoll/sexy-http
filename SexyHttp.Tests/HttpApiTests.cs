using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SexyHttp.Tests
{
    [TestFixture]
    public class HttpApiTests
    {
        [Test]
        public void Get()
        {
            var api = new HttpApi<IGet>();
            Assert.AreEqual(HttpMethod.Get, api.Endpoints.Single().Method);
        }

        [Test]
        public void Path()
        {
            var api = new HttpApi<IGet>();
            Assert.AreEqual("http://localhost/path", api.Endpoints.Single().Path.CreateUrl("http://localhost").ToString());
        }

        interface IGet
        {
            [Get("path")]
            Task<string> GetString();
        }

        [Test]
        public void Post()
        {
            var api = new HttpApi<IPost>();
            Assert.AreEqual(HttpMethod.Post, api.Endpoints.Single().Method);
        }

        interface IPost
        {
            [Post("path")]
            Task Post();
        }

        [Test]
        public async void PathWithSubstitution()
        {
            var api = new HttpApi<IPathWithSubstitution>();
            var endpoint = api.Endpoints.Single();
            var httpHandler = new MockHttpHandler();
            await endpoint.Call(httpHandler, new MockHeadersProvider(), "http://localhost", new Dictionary<string, object> { { "key", "to" } });
            Assert.AreEqual("http://localhost/path/to/api", httpHandler.Request.Url.ToString());
        }

        interface IPathWithSubstitution
        {
            [Get("path/{key}/api")]
            Task<string> GetString(string key);
        }
    }
}