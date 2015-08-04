using NUnit.Framework;
using SexyHttp.Urls;

namespace SexyHttp.Tests
{
    [TestFixture]
    public class HttpUrlParserTests
    {
        [Test]
        public void Literal()
        {
            var path = HttpUrlParser.Parse("path/to/api").CreateUrl("http://localhost");
            Assert.AreEqual("http://localhost/path/to/api", path.ToString());
        }

        [Test]
        public void Variable()
        {
            var descriptor = HttpUrlParser.Parse("{key}");
            var url = descriptor.CreateUrl("http://localhost");
            url.Path["key"] = "to";
            Assert.AreEqual("http://localhost/to", url.ToString());
        }

        [Test]
        public void FullPath()
        {
            var descriptor = HttpUrlParser.Parse("path/{key}/api");
            var url = descriptor.CreateUrl("http://localhost");
            url.Path["key"] = "to";
            Assert.AreEqual("http://localhost/path/to/api", url.ToString());
        }

        [Test]
        public void QueryLiteral()
        {
            var url = HttpUrlParser.Parse("?key=value").CreateUrl("http://localhost");
            Assert.AreEqual("http://localhost?key=value", url.ToString());
        }

        [Test]
        public void QueryVariable()
        {
            var descriptor = HttpUrlParser.Parse("?key={variable}");
            var url = descriptor.CreateUrl("http://localhost");
            url.Query["variable"] = new[] { "value" };
            Assert.AreEqual("http://localhost?key=value", url.ToString());
        }

        [Test]
        public void QueryFullUrl()
        {
            var descriptor = HttpUrlParser.Parse("path/{key1}/api?key2={variable}");
            var url = descriptor.CreateUrl("http://localhost");
            url.Path["key1"] = "to";
            url.Query["variable"] = new[] { "value2" };
            Assert.AreEqual("http://localhost/path/to/api?key2=value2", url.ToString());
        }
    }
}