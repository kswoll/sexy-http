using NUnit.Framework;
using SexyHttp.Urls;

namespace SexyHttp.Tests
{
    [TestFixture]
    public class HttpPathTests
    {
        [Test]
        public void Literal()
        {
            var path = new HttpUrlDescriptor("foo").CreateUrl("http://localhost");
            Assert.AreEqual("http://localhost/foo", path.ToString());
        }

        [Test]
        public void Variable()
        {
            var variable = new VariableHttpPathPart("key");
            var url = new HttpUrlDescriptor(variable).CreateUrl("http://localhost");
            url.Path["key"] = "foo";
            Assert.AreEqual("http://localhost/foo", url.ToString());
        }

        [Test]
        public void Mix()
        {
            var variable = new VariableHttpPathPart("key");
            var url = new HttpUrlDescriptor("path/", variable, "/api").CreateUrl("http://localhost");
            url.Path["key"] = "to";
            Assert.AreEqual("http://localhost/path/to/api", url.ToString());
        }
    }
}