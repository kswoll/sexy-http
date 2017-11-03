using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SexyHttp.Mocks;
using SexyHttp.TypeConverters;

namespace SexyHttp.Tests
{
    [TestFixture]
    public class BodyBasedTypeConvertersTests
    {
        [Test]
        public async void Query()
        {
            var api = new HttpApi<IQueryApi>();
            var endpoint = api.Endpoints.Single().Value;
            var httpHandler = new MockHttpHandler();
            await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object> { ["id"] = 1, ["query"] = 2 });
            Assert.AreEqual("http://localhost/1?foo=bar", httpHandler.Request.Url.ToString());
        }

        [TypeConverter(typeof(TestConverter), TypeConversionContext.Query)]
        private interface IQueryApi
        {
            [Get("{id}?foo={query}")]
            Task Test(int id, int query);
        }

        private class TestConverter : ITypeConverter
        {
            public bool TryConvertTo(ITypeConverter root, TypeConversionContext context, Type convertTo, object value, out object result)
            {
                if (convertTo == typeof(string))
                {
                    result = "bar";
                    return true;
                }
                result = null;
                return false;
            }
        }
    }
}