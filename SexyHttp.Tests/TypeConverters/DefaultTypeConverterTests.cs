using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using SexyHttp.TypeConverters;

namespace SexyHttp.Tests.TypeConverters
{
    [TestFixture]
    public class DefaultTypeConverterTests
    {
        [Test]
        public void SourceBaseType()
        {
            var registry = DefaultTypeConverter.Create();

            var result1 = registry.ConvertTo<string>(TypeConversionContext.None, TestEnum.Value1);
            var result2 = registry.ConvertTo<string>(TypeConversionContext.None, TestEnum.Value2);
            var result3 = registry.ConvertTo<string>(TypeConversionContext.None, true);
            Assert.AreEqual("value1", result1);
            Assert.AreEqual("Value2", result2);
            Assert.AreEqual("true", result3);
        }

        [Test]
        public void TargetBaseType()
        {
            var registry = DefaultTypeConverter.Create();

            var result1 = registry.ConvertTo<TestEnum>(TypeConversionContext.None, "value1");
            var result2 = registry.ConvertTo<TestEnum>(TypeConversionContext.None, "Value2");
            var result3 = registry.ConvertTo<bool>(TypeConversionContext.None, "true");
            var result4 = registry.ConvertTo<bool>(TypeConversionContext.None, "True");
            Assert.AreEqual(TestEnum.Value1, result1);
            Assert.AreEqual(TestEnum.Value2, result2);
            Assert.AreEqual(true, result3);
            Assert.AreEqual(true, result4);
        }

        public enum TestEnum {[EnumMember(Value = "value1")] Value1, Value2 }

    }
}