using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
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
            await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object> { ["key"] = "to" }, new MockApiRequestInstrumenter());
            Assert.AreEqual("http://localhost/path/to/api", httpHandler.Request.Url.ToString());
        }

        interface IPathWithSubstitution
        {
            [Get("path/{key}/api")]
            Task<string> GetString(string key);
        }

        [Test]
        public async void QueryWithSubstitution()
        {
            var api = new HttpApi<IQueryWithSubstitution>();
            var endpoint = api.Endpoints.Single().Value;
            var httpHandler = new MockHttpHandler();
            await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object> { ["key"] = "bar" }, new MockApiRequestInstrumenter());
            Assert.AreEqual("http://localhost/path?foo=bar", httpHandler.Request.Url.ToString());
        }

        interface IQueryWithSubstitution
        {
            [Get("path?foo={key}")]
            Task<string> GetString(string key);
        }

        [Test]
        public async void ApiLevelTypeConverter()
        {
            var api = new HttpApi<IApiLevelTypeConverter>();
            var endpoint = api.Endpoints.Single().Value;
            var httpHandler = new MockHttpHandler();
            await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object> { ["number"] = 5 }, new MockApiRequestInstrumenter());
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
            await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object> { ["number"] = 5 }, new MockApiRequestInstrumenter());
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
            await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object> { ["number"] = 5 }, new MockApiRequestInstrumenter());
            Assert.AreEqual("http://localhost/foo", httpHandler.Request.Url.ToString());                        
        }

        interface IParameterLevelTypeConverter
        {
            [Post("{number}")]
            Task PostInt([TypeConverter(typeof(TestTypeConverter))]int number);            
        }

        class TestTypeConverter : ITypeConverter
        {
            public bool TryConvertTo<T>(object obj, out T result)
            {
                result = (T)(object)"foo";
                return true;
            }
        }

        [Test]
        public async Task DirectJsonBody()
        {
            var api = new HttpApi<IDirectJsonBody>();
            var endpoint = api.Endpoints.Single().Value;
            var httpHandler = new MockHttpHandler();
            await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object> { ["number"] = 5 }, new MockApiRequestInstrumenter());

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
            await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object> { ["value1"] = 5, ["value2"] = "foo" }, new MockApiRequestInstrumenter());

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
            var result = (string)await endpoint.Call(httpHandler, "http://localhost", new Dictionary<string, object>(), new MockApiRequestInstrumenter());
            Assert.AreEqual("foo", result);
        }

        interface IJsonResponse
        {
            [Get("path")]
            Task<string> GetString();
        }
    }
}