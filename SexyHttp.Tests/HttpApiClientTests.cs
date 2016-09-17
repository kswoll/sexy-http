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
            var client = HttpApiClient<IApi>.Create("http://localhost", httpHandler);
            var result = await client.GetString("foo");
            Assert.AreEqual("foo", result);
        }

        [Proxy]
        public interface IApi
        {
            [Get("path")]
            Task<string> GetString(string argument);
        }

        [Test]
        public void InterfaceHandlerForPropertyGet()
        {
            var httpHandler = new MockHttpHandler(x => new HttpApiResponse(body: x.Body));
            var accessToken = "foo";
            var client = HttpApiClient<IInterfaceHandlerApi>.Create("http://localhost", httpHandler, interfaceHandler: invocation =>
            {
                switch (invocation.Method.Name)
                {
                    case "get_AccessToken":
                        return Task.FromResult((object)accessToken);
                }
                return invocation.Proceed();
            });
            var result = client.AccessToken;
            Assert.AreEqual("foo", result);
        }

        [Proxy]
        public interface IInterfaceHandlerApi
        {
            string AccessToken { get; }
        }
    }
}