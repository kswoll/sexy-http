using System.Collections.Generic;
using NUnit.Framework;

namespace SexyHttp.Tests
{
    [TestFixture]
    public class HttpPathTests
    {
        [Test]
        public void Literal()
        {
            var path = new HttpPath("foo");
            Assert.AreEqual("foo", path.ToString(new Dictionary<string, object>()));
        }

        [Test]
        public void Variable()
        {
            var path = new HttpPath(new VariableHttpPathPart("key"));
            Assert.AreEqual("foo", path.ToString(new Dictionary<string, object> { { "key", "foo" } }));
        }

        [Test]
        public void Mix()
        {
            var path = new HttpPath("path/", new VariableHttpPathPart("key"), "/api");
            Assert.AreEqual("path/to/api", path.ToString(new Dictionary<string, object> { { "key", "to" } }));
        }
    }
}