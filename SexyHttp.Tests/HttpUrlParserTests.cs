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
    }
}