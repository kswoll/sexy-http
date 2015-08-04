using System.Collections.Generic;
using NUnit.Framework;
using SexyHttp.Urls;
using System.Linq;

namespace SexyHttp.Tests
{
    [TestFixture]
    public class HttpUrlTests
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

        [Test]
        public void LiteralQuery()
        {
            var url = new HttpUrlDescriptor(new KeyValuePair<string, HttpUrlPart>("key", "value")).CreateUrl("http://localhost");
            Assert.AreEqual("http://localhost?key=value", url.ToString());
        }

        [Test]
        public void VariableQueryOneValue()
        {
            var variable = new VariableHttpPathPart("variable");
            var url = new HttpUrlDescriptor(new KeyValuePair<string, HttpUrlPart>("key", variable)).CreateUrl("http://localhost");
            url.Query["variable"] = new[] { "value" };
            Assert.AreEqual("http://localhost?key=value", url.ToString());
        }

        [Test]
        public void VariableQueryTwoValues()
        {
            var variable = new VariableHttpPathPart("variable");
            var url = new HttpUrlDescriptor(new KeyValuePair<string, HttpUrlPart>("key", variable)).CreateUrl("http://localhost");
            url.Query["variable"] = new[] { "value1", "value2" };
            Assert.AreEqual("http://localhost?key=value1&key=value2", url.ToString());
        }

        [Test]
        public void VariableQueryTwoPairs()
        {
            var variable1 = new VariableHttpPathPart("variable1");
            var variable2 = new VariableHttpPathPart("variable2");
            var url = new HttpUrlDescriptor(new KeyValuePair<string, HttpUrlPart>("key1", variable1), 
                new KeyValuePair<string, HttpUrlPart>("key2", variable2)).CreateUrl("http://localhost");
            url.Query["variable1"] = new[] { "value1" };
            url.Query["variable2"] = new[] { "value2" };
            Assert.AreEqual("http://localhost?key1=value1&key2=value2", url.ToString());
        }

        [Test]
        public void FullUrl()
        {
            var variable1 = new VariableHttpPathPart("key1");
            var variable2 = new VariableHttpPathPart("variable2");
            var url = new HttpUrlDescriptor(new[] { variable1 }, new[] { new KeyValuePair<string, HttpUrlPart>("key2", variable2) }).CreateUrl("http://localhost");
            url.Path["key1"] = "value1";
            url.Query["variable2"] = new[] { "value2" };
            Assert.AreEqual("http://localhost/value1?key2=value2", url.ToString());
        }
    }
}