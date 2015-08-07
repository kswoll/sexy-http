using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SexyHttp.HttpHandlers;

namespace SexyHttp.Tests
{
    [TestFixture]
    public class EmitBasedClientTests
    {
        [Test]
        public async void GetString()
        {
            using (MockHttpServer.ReturnJson(request => Task.FromResult<JToken>(new JValue("foo"))))
            {
                var client = HttpApiClient<IGetString>.Create("http://localhost:8844/path", new HttpClientHandler());
                var result = await client.GetString();
                Assert.AreEqual("foo", result);
            }
        }

        public interface IGetString
        {
            [Get("path")]
            Task<string> GetString();
        }
    }
}
