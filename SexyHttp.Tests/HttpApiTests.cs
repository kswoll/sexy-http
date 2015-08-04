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
    }
}