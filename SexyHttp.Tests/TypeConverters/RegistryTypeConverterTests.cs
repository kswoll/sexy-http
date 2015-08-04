using NUnit.Framework;
using SexyHttp.TypeConverters;

namespace SexyHttp.Tests.TypeConverters
{
    [TestFixture]
    public class RegistryTypeConverterTests
    {
        [Test]
        public void DirectConversion()
        {
            var registry = new RegistryTypeConverter();
            registry.Register(typeof(string), typeof(int), LambdaTypeConverter.Create(x => int.Parse((string)x)));

            var result = registry.ConvertTo<int>("5");
            Assert.AreEqual(5, result);
        }
    }
}