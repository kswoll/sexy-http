using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SexyHttp.HttpBodies;
using SexyHttp.TypeConverters;

namespace SexyHttp.Tests
{
    [TestFixture]
    public class HttpApiTests
    {
        [Test]
        public void Get()
        {
            var api = new HttpApi<IGet>();
            Assert.AreEqual(HttpMethod.Get, api.Endpoints.Single().Value.Method);
        }

        [Test]
        public void Path()
        {
            var api = new HttpApi<IGet>();
            Assert.AreEqual("http://localhost/path", api.Endpoints.Single().Value.Path.CreateUrl("http://localhost").ToString());
        }

        interface IGet
        {
            [Get("path")]
            Task<string> GetString();
        }

        [Test]
        public void EmptyPath()
        {
            var api = new HttpApi<IGetEmptyPath>();
            Assert.AreEqual("http://localhost", api.Endpoints.Single().Value.Path.CreateUrl("http://localhost").ToString());
        }

        interface IGetEmptyPath
        {
            [Get]
            Task<string> GetString();
        }

        [Test]
        public void Post()
        {
            var api = new HttpApi<IPost>();
            Assert.AreEqual(HttpMethod.Post, api.Endpoints.Single().Value.Method);
        }

        interface IPost
        {
            [Post("path")]
            Task Post();
        }

        [Test]
        public async void PathWithSubstitution()
        {
            var api = new HttpApi<IPathWithSubstitution>();
            var endpoint = api.Endpoints.Single().Value;
            var httpHandler = new MockHttpHandler();
            await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object> { ["key"] = "to" });
            Assert.AreEqual("http://localhost/path/to/api", httpHandler.Request.Url.ToString());
        }

        interface IPathWithSubstitution
        {
            [Get("path/{key}/api")]
            Task<string> GetString(string key);
        }

        [Test]
        public async void PathForApiType()
        {
            var api = new HttpApi<IPathForApi>();
            var endpoint = api.Endpoints.Single().Value;
            var httpHandler = new MockHttpHandler();
            await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object> { ["key"] = "to" });
            Assert.AreEqual("http://localhost/path/to/api", httpHandler.Request.Url.ToString());
        }

        [Path("path")]
        interface IPathForApi
        {
            [Get("{key}/api")]
            Task<string> GetString(string key);
        }

        [Test]
        public async void QueryWithSubstitution()
        {
            var api = new HttpApi<IQueryWithSubstitution>();
            var endpoint = api.Endpoints.Single().Value;
            var httpHandler = new MockHttpHandler();
            await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object> { ["key"] = "bar" });
            Assert.AreEqual("http://localhost/path?foo=bar", httpHandler.Request.Url.ToString());
        }

        interface IQueryWithSubstitution
        {
            [Get("path?foo={key}")]
            Task<string> GetString(string key);
        }

        [Test]
        public async void QueryWithInt()
        {
            var api = new HttpApi<IQueryWithInt>();
            var endpoint = api.Endpoints.Single().Value;
            var httpHandler = new MockHttpHandler();
            await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object> { ["key"] = 5 });
            Assert.AreEqual("http://localhost/path?foo=5", httpHandler.Request.Url.ToString());
        }

        interface IQueryWithInt
        {
            [Get("path?foo={key}")]
            Task<string> GetString(int key);
        }

        [Test]
        public async void MultipleQueryArguments()
        {
            var api = new HttpApi<IMultipeQueryArguments>();
            var endpoint = api.Endpoints.Single().Value;
            var httpHandler = new MockHttpHandler();
            await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object> { ["ids"] = new[] { 1, 3 } });
            Assert.AreEqual("http://localhost?ids=1&ids=3", httpHandler.Request.Url.ToString());            
            await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object> { ["firstName"] = "John" });
            Assert.AreEqual("http://localhost?firstName=John", httpHandler.Request.Url.ToString());            
        }

        interface IMultipeQueryArguments
        {
            [Get("?ids={ids}&firstName={firstName}&lastName={lastName}")]
            Task<User[]> Find(int[] ids = null, string firstName = null, string lastName = null);
        }

        [Test]
        public async void MultipelIdsCommaSeparated()
        {
            var api = new HttpApi<IMultipleIdsCommaSeparated>();
            var endpoint = api.Endpoints.Single().Value;
            var httpHandler = new MockHttpHandler();
            await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object> { ["ids"] = new[] { 1, 3 } });
            Assert.AreEqual("http://localhost?ids=1,3", httpHandler.Request.Url.ToString());            
        }

        [TypeConverter(typeof(ArrayAsCommaSeparatedStringConverter))]
        interface IMultipleIdsCommaSeparated
        {
            [Get("?ids={ids}&firstName={firstName}&lastName={lastName}")]
            Task<User[]> Find(int[] ids = null, string firstName = null, string lastName = null);
        }

        private class User
        {
        }

        [Test]
        public async void ApiLevelTypeConverter()
        {
            var api = new HttpApi<IApiLevelTypeConverter>();
            var endpoint = api.Endpoints.Single().Value;
            var httpHandler = new MockHttpHandler();
            await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object> { ["number"] = 5 });
            Assert.AreEqual("http://localhost/foo", httpHandler.Request.Url.ToString());
        }

        [TypeConverter(typeof(TestTypeConverter))]
        interface IApiLevelTypeConverter
        {
            [Post("{number}")]
            Task PostInt(int number);
        }

        [Test]
        public async void EndpointLevelTypeConverter()
        {
            var api = new HttpApi<IEndpointLevelTypeConverter>();
            var endpoint = api.Endpoints.Single().Value;
            var httpHandler = new MockHttpHandler();
            await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object> { ["number"] = 5 });
            Assert.AreEqual("http://localhost/foo", httpHandler.Request.Url.ToString());            
        }

        interface IEndpointLevelTypeConverter
        {
            [Post("{number}"), TypeConverter(typeof(TestTypeConverter))]
            Task PostInt(int number);
        }

        [Test]
        public async void ParameterLevelTypeConverter()
        {
            var api = new HttpApi<IParameterLevelTypeConverter>();
            var endpoint = api.Endpoints.Single().Value;
            var httpHandler = new MockHttpHandler();
            await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object> { ["number"] = 5 });
            Assert.AreEqual("http://localhost/foo", httpHandler.Request.Url.ToString());                        
        }

        interface IParameterLevelTypeConverter
        {
            [Post("{number}")]
            Task PostInt([TypeConverter(typeof(TestTypeConverter))]int number);            
        }

        [Test]
        public async void ReturnTypeConverter()
        {
            var api = new HttpApi<IReturnTypeConverter>();
            var endpoint = api.Endpoints.Single().Value;
            var httpHandler = new MockHttpHandler();
            var result = await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object> { ["number"] = 5 });
            Assert.AreEqual("foo", result);
        }

        interface IReturnTypeConverter
        {
            [Post]
            [return: TypeConverter(typeof(TestTypeConverter))]Task<string> Post();
        }

        class TestTypeConverter : ITypeConverter
        {
            public bool TryConvertTo(ITypeConverter root, TypeConversionContext context, Type convertTo, object value, out object result)
            {
                if (convertTo == typeof(string))
                {
                    result = "foo";
                    return true;                    
                }
                result = null;
                return false;
            }
        }

        [Test]
        public async Task DirectJsonBody()
        {
            var api = new HttpApi<IDirectJsonBody>();
            var endpoint = api.Endpoints.Single().Value;
            var httpHandler = new MockHttpHandler();
            await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object> { ["number"] = 5 });

            var body = (JsonHttpBody)httpHandler.Request.Body;
            var value = (JValue)body.Json;
            Assert.AreEqual(5, (int)value);
        }

        interface IDirectJsonBody
        {
            [Post("path")]
            Task PostInt(int number);
        }

        [Test]
        public async Task ComposedJsonBody()
        {
            var api = new HttpApi<IComposedJsonBody>();
            var endpoint = api.Endpoints.Single().Value;
            var httpHandler = new MockHttpHandler();
            await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object> { ["value1"] = 5, ["value2"] = "foo" });

            var body = (JsonHttpBody)httpHandler.Request.Body;
            var jsonObject = (JObject)body.Json;
            Assert.AreEqual(5, (int)jsonObject["value1"]);            
            Assert.AreEqual("foo", (string)jsonObject["value2"]);
        }

        interface IComposedJsonBody
        {
            [Post("path")]
            Task PostTwoValues(int value1, string value2);
        }

        [Test]
        public async void JsonResponse()
        {
            var api = new HttpApi<IJsonResponse>();
            var endpoint = api.Endpoints.Single().Value;
            var httpHandler = new MockHttpHandler(new HttpApiResponse(body: new JsonHttpBody("foo")));
            var result = (string)await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object>());
            Assert.AreEqual("foo", result);
        }

        interface IJsonResponse
        {
            [Get("path")]
            Task<string> GetString();
        }
    }
}