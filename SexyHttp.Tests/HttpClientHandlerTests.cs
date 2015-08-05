using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SexyHttp.HttpHandlers.HttpClientHandlers;
using SexyProxy;

namespace SexyHttp.Tests
{
    [TestFixture]
    public class HttpClientHandlerTests 
    {
        [Test]
        public async void GetString()
        {
            using (new MockHttpServer(request => new JValue("foo")))
            {
                var client = HttpApiClient<IGetString>.Create("http://localhost:8844", new HttpClientHandler());
                await client.GetString();
            }
        }

        [Proxy]
        private interface IGetString
        {
            [Get("path")]
            Task<string> GetString();
        }
    }
}