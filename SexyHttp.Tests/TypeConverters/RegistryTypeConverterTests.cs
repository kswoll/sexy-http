using System;
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

        [Test]
        public void SourceBaseType()
        {
            var registry = new RegistryTypeConverter();
            registry.Register<Enum, string>(LambdaTypeConverter.Create(x => x.ToString()));

            var result = registry.ConvertTo<string>(TestEnum.Value1);
            Assert.AreEqual("Value1", result);
        }

        [Test]
        public void TargetBaseType()
        {
            var registry = new RegistryTypeConverter();
            registry.Register<string, Enum>(LambdaTypeConverter.Create((x, convertTo) => Enum.Parse(convertTo, (string)x)));

            var result = registry.ConvertTo<TestEnum>("Value1");
            Assert.AreEqual(TestEnum.Value1, result);
        }

        public enum TestEnum { Value1, Value2 }
    }
}