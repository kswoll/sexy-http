using NUnit.Framework;

namespace SexyHttp.Tests
{
    [TestFixture]
    public class HttpPathTests
    {
        [Test]
        public void Literal()
        {
            var path = new HttpPathDescriptor("foo").CreatePath();
            Assert.AreEqual("foo", path.ToString());
        }

        [Test]
        public void Variable()
        {
            var variable = new VariableHttpPathPart("key");
            var path = new HttpPathDescriptor(variable).CreatePath();
            path[variable] = "foo";
            Assert.AreEqual("foo", path.ToString());
        }

        [Test]
        public void Mix()
        {
            var variable = new VariableHttpPathPart("key");
            var path = new HttpPathDescriptor("path/", variable, "/api").CreatePath();
            path[variable] = "to";
            Assert.AreEqual("path/to/api", path.ToString());
        }
    }
}