using System.Threading.Tasks;
using NUnit.Framework;
using SexyProxy;

namespace SexyHttp.Tests
{
    [TestFixture]
    public class HttpApiClientTests
    {
        [Test]
        public async void GetString()
        {
            var httpHandler = new MockHttpHandler(x => new HttpApiResponse(body: x.Body));
            var client = HttpApiClient<IApi>.Create(new HttpApi<IApi>(), "http://localhost", httpHandler, new MockApiRequestInstrumenter());
            var result = await client.GetString("foo");
            Assert.AreEqual("foo", result);
        }

        [Proxy]
        public interface IApi 
        {
            [Get("path")]
            Task<string> GetString(string argument);
        }
    }
}