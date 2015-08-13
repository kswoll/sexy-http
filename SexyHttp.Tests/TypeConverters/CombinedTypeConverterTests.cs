using NUnit.Framework;
using SexyHttp.TypeConverters;

namespace SexyHttp.Tests.TypeConverters
{
    [TestFixture]
    public class CombinedTypeConverterTests
    {
        [Test]
        public void FirstChoice()
        {
            var typeConverter = new CombinedTypeConverter(
                LambdaTypeConverter.Create(x =>
                {
                    int result;
                    return new TypeConverterResult<int>(int.TryParse((string)x, out result), result);
                }),
                LambdaTypeConverter.Create(x =>
                {
                    double result;
                    return new TypeConverterResult<double>(double.TryParse((string)x, out result), result);
                }));

            var value = typeConverter.ConvertTo<int>(TypeConversionContext.None, "5");
            Assert.AreEqual(5, value);
        }

        [Test]
        public void SecondChoice()
        {
            var typeConverter = new CombinedTypeConverter(
                LambdaTypeConverter.Create(x =>
                {
                    int result;
                    return new TypeConverterResult<int>(int.TryParse((string)x, out result), result);
                }),
                LambdaTypeConverter.Create(x =>
                {
                    double result;
                    return new TypeConverterResult<double>(double.TryParse((string)x, out result), result);
                }));

            var value = typeConverter.ConvertTo<double>(TypeConversionContext.None, "5.5");
            Assert.AreEqual(5.5D, value);
        }
    }
}