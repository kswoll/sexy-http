using NUnit.Framework;

namespace SexyHttp.Tests
{
    [TestFixture]
    public class HttpPathParserTests
    {
        [Test]
        public void Literal()
        {
            var path = HttpPathParser.Parse("path/to/api").CreatePath();
            Assert.AreEqual("path/to/api", path.ToString());
        }

        [Test]
        public void Variable()
        {
            var descriptor = HttpPathParser.Parse("{key}");
            var variable = descriptor.Parts[0];
            var path = descriptor.CreatePath();
            path[variable] = "to";
            Assert.AreEqual("to", path.ToString());
        }

        [Test]
        public void FullPath()
        {
            var descriptor = HttpPathParser.Parse("path/{key}/api");
            var variable = descriptor.Parts[1];
            var path = descriptor.CreatePath();
            path[variable] = "to";
            Assert.AreEqual("path/to/api", path.ToString());
        }
    }
}