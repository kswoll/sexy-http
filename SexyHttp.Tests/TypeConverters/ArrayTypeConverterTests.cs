using NUnit.Framework;
using SexyHttp.TypeConverters;

namespace SexyHttp.Tests.TypeConverters
{
    [TestFixture]
    public class ArrayTypeConverterTests
    {
        [Test]
        public void ToArray()
        {
            var typeConverter = new ArrayTypeConverter();
            var result = typeConverter.ConvertTo<string[]>("foo");
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("foo", result[0]);
        }
    }
}