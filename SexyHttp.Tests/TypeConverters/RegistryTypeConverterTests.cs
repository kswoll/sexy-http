using System;
using System.Linq;
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

            var result = registry.ConvertTo<int>(TypeConversionContext.None, "5");
            Assert.AreEqual(5, result);
        }

        [Test]
        public void SourceBaseType()
        {
            var registry = new RegistryTypeConverter();
            registry.Register<Enum, string>(LambdaTypeConverter.Create(x => x.ToString()));

            var result = registry.ConvertTo<string>(TypeConversionContext.None, TestEnum.Value1);
            Assert.AreEqual("Value1", result);
        }

        [Test]
        public void TargetBaseType()
        {
            var registry = new RegistryTypeConverter();
            registry.Register<string, Enum>(LambdaTypeConverter.Create((x, convertTo) => Enum.Parse(convertTo, (string)x)));

            var result = registry.ConvertTo<TestEnum>(TypeConversionContext.None, "Value1");
            Assert.AreEqual(TestEnum.Value1, result);
        }

        [Test]
        public void ArrayToBaseElementType()
        {
            var registry = new RegistryTypeConverter();
            registry.Register<Array, string[]>(LambdaTypeConverter.Create(value => ((Array)value).Cast<object>().Select(x => x.ToString()).ToArray()));

            var result = registry.ConvertTo<string[]>(TypeConversionContext.None, new[] { 2, 3 });
            Assert.AreEqual("2", result[0]);
            Assert.AreEqual("3", result[1]);
        }

        public enum TestEnum { Value1, Value2 }
    }
}