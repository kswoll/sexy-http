using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SexyHttp.Tests
{
    [TestFixture]
    public class HttpPathParserTests
    {
        [Test]
        public void Literal()
        {
            var path = HttpPathParser.Parse("path/to/api");
            Assert.AreEqual("path/to/api", path.ToString(new Dictionary<string, object>()));
        }

        [Test]
        public void Variable()
        {
            var path = HttpPathParser.Parse("{key}");
            Assert.AreEqual("to", path.ToString(new Dictionary<string, object> { { "key", "to" } }));
        }

        [Test]
        public void FullPath()
        {
            var path = HttpPathParser.Parse("path/{key}/api");
            Assert.AreEqual("path/to/api", path.ToString(new Dictionary<string, object> { { "key", "to" } }));
        }
    }
}