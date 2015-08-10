using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SexyHttp.HttpBodies;
using SexyHttp.HttpHandlers;
using SexyHttp.Tests.Utils;
using SexyProxy;

namespace SexyHttp.Tests
{
    [TestFixture]
    public class HttpClientHandlerTests 
    {
        [Test]
        public async void GetString()
        {
            using (MockHttpServer.ReturnJson(request => Task.FromResult<JToken>(new JValue("foo"))))
            {
                var client = HttpApiClient<IGetString>.Create("http://localhost:8844/path", new HttpClientHandler());
                await client.GetString();
            }
        }

        [Proxy]
        private interface IGetString
        {
            [Get("path")]
            Task<string> GetString();
        }

        [Test]
        public async void ReflectValue()
        {
            using (MockHttpServer.Json(x => x))
            {
                var client = HttpApiClient<IReflectValue>.Create("http://localhost:8844/path", new HttpClientHandler());
                var result = await client.ReflectValue("foo");
                Assert.AreEqual("foo", result);
            }
        }

        [Proxy]
        private interface IReflectValue
        {
            [Post("path")]
            Task<string> ReflectValue([Value]string argument);
        }

        [Test]
        public async void PostStringMultipart()
        {
            using (MockHttpServer.PostMultipartReturnJson(x => Task.FromResult<JToken>(((StringHttpBody)x.Data["value"].Body).Text)))
            {
                var client = HttpApiClient<IPostStringMultipart>.Create("http://localhost:8844/path", new HttpClientHandler());
                var result = await client.PostString("foo");
                Assert.AreEqual("foo", result);
            }            
        }

        [Proxy]
        private interface IPostStringMultipart
        {
            [Post("path"), Multipart]
            Task<string> PostString(string value);
        }

        [Test]
        public async void PostByteArrayMultipart()
        {
            using (MockHttpServer.PostMultipartReturnByteArray(x => Task.FromResult(((ByteArrayHttpBody)x.Data["data"].Body).Data)))
            {
                var input = new byte[] { 3, 1, 8, 9, 15 };
                var client = HttpApiClient<IPostByteArrayMultipart>.Create("http://localhost:8844/path", new HttpClientHandler());
                var result = await client.PostByteArray(input);
                Assert.IsTrue(result.SequenceEqual(input));
            }            
        }

        [Proxy]
        private interface IPostByteArrayMultipart
        {
            [Post("path"), Multipart]
            Task<byte[]> PostByteArray(byte[] data);
        }

        [Test]
        public async void PostStream()
        {
            using (MockHttpServer.PostStreamReturnByteArray(async x => await x.ReadToEndAsync()))
            {
                var input = new byte[] { 3, 1, 6, 9, 38 };
                var client = HttpApiClient<IPostStream>.Create("http://localhost:8844/path", new HttpClientHandler());
                var result = await client.PostStream(new MemoryStream(input));
                Assert.IsTrue(result.SequenceEqual(input));
            }
        }

        [Proxy]
        private interface IPostStream
        {
            [Post("path")]
            Task<byte[]> PostStream(Stream stream);
        }

        [Test]
        public async void PostTwoStreams()
        {
            using (MockHttpServer.PostMultipartStreamReturnJson(x => Task.FromResult<JToken>(
                ((ByteArrayHttpBody)x.Data["stream1"].Body).Data.Length +
                ((ByteArrayHttpBody)x.Data["stream2"].Body).Data.Length)))
            {
                var input1 = new byte[] { 3, 1, 6, 9, 38 };
                var input2 = new byte[] { 2, 5, 13, 7 };
                var client = HttpApiClient<IPostTwoStreams>.Create("http://localhost:8844/path", new HttpClientHandler());
                var result = await client.PostTwoStreams(new MemoryStream(input1), new MemoryStream(input2));
                Assert.AreEqual(9, result);
            }            
        }

        [Proxy]
        private interface IPostTwoStreams
        {
            [Post("path")]
            Task<int> PostTwoStreams(Stream stream1, Stream stream2);
        }

        [Test]
        public async void DownloadStream()
        {
            var data = new byte[] { 3, 1, 4, 5 };
            using (MockHttpServer.ReturnByteArray(x => data))
            {
                var client = HttpApiClient<IDownloadStream>.Create("http://localhost:8844/path", new HttpClientHandler());
                byte[] result = null;
                await client.DownloadStream(async x => result = await x.ReadToEndAsync());
                Assert.IsTrue(result.SequenceEqual(data));
            }
        }

        [Proxy]
        private interface IDownloadStream
        {
            [Get("path")]
            Task DownloadStream(Func<Stream, Task> stream);
        }

        [Test]
        public async void PostForm()
        {
            using (MockHttpServer.PostFormReturnJson(x => Task.FromResult<JToken>(x.Values["value1"] + "|" + x.Values["value2"])))
            {
                var client = HttpApiClient<IPostForm>.Create("http://localhost:8844/path", new HttpClientHandler());
                var result = await client.PostForm("value&1", 5);
                Assert.AreEqual("value&1|5", result);
            }            
        }

        [Proxy]
        private interface IPostForm
        {
            [Post("path"), Form]
            Task<string> PostForm(string value1, int value2);
        }

        [Test]
        public async void ReceiveForm()
        {
            using (MockHttpServer.PostFormReturnForm(x => Task.FromResult(x)))
            {
                var client = HttpApiClient<IReceiveForm>.Create("http://localhost:8844/path", new HttpClientHandler());
                var result = await client.GetForm("value&1", 5);
                Assert.AreEqual("value&1", result.Value1);
                Assert.AreEqual(5, result.Value2);
            }            
        }

        [Proxy]
        private interface IReceiveForm
        {
            [Post("path"), Form]
            Task<FormResponse> GetForm(string value1, int value2);
        }

        private class FormResponse
        {
            public string Value1 { get; set; }
            public int Value2 { get; set; }
        }

        [Test]
        public async void JsonNameOverride()
        {
            using (MockHttpServer.Json(x => (string)x["val1"] + x["val2"]))
            {
                var client = HttpApiClient<INameOverride>.Create("http://localhost:8844/path", new HttpClientHandler());
                var result = await client.GetString("foo", "bar");
                Assert.AreEqual("foobar", result);
            }            
        }

        [Proxy]
        interface INameOverride
        {
            [Post("path")]
            Task<string> GetString([Name("val1")]string value1, [Name("val2")]string value2);
        }

        [Test]
        public async void FormNameOverride()
        {
            using (MockHttpServer.PostFormReturnJson(x => x.Values["val1"] + x.Values["val2"]))
            {
                var client = HttpApiClient<IFormNameOverride>.Create("http://localhost:8844/path", new HttpClientHandler());
                var result = await client.GetString("foo", "bar");
                Assert.AreEqual("foobar", result);
            }            
        }

        [Proxy]
        interface IFormNameOverride
        {
            [Post("path"), Form]
            Task<string> GetString([Name("val1")]string value1, [Name("val2")]string value2);
        }

        [Test]
        public async void SingleArgumentAsObject()
        {
            using (MockHttpServer.Json(x => x["value"]))
            {
                var client = HttpApiClient<ISingleArgumentAsObject>.Create("http://localhost:8844/path", new HttpClientHandler());
                var result = await client.GetString("foo");
                Assert.AreEqual("foo", result);
            }            
        }

        [Proxy]
        interface ISingleArgumentAsObject
        {
            [Post("path")]
            Task<string> GetString(string value);
        }

        [Test]
        public async void RawRequestApi()
        {
            using (MockHttpServer.Json(x => x))
            {
                var client = HttpApiClient<IRawRequestApi>.Create("http://localhost:8844/path", new HttpClientHandler());
                var result = await client.GetString(x => x.Body = new JsonHttpBody("foo"));
                Assert.AreEqual("foo", result);
            }            
        }

        [Proxy]
        interface IRawRequestApi
        {
            [Post("path")]
            Task<string> GetString(Action<HttpApiRequest> request);
        }

        [Test]
        public async void RawResponseApi()
        {
            using (MockHttpServer.Json(x => x))
            {
                var client = HttpApiClient<IRawResponseApi>.Create("http://localhost:8844/path", new HttpClientHandler());
                var result = await client.GetString("foo");
                Assert.AreEqual("foo", (string)((JsonHttpBody)result.Body).Json);
            }            
        }

        [Proxy]
        interface IRawResponseApi
        {
            [Post("path")]
            Task<HttpApiResponse> GetString([Value]string value);
        }

        [Test]
        public async void RawRequestBody()
        {
            using (MockHttpServer.Json(x => x))
            {
                var client = HttpApiClient<IRawRequestBody>.Create("http://localhost:8844/path", new HttpClientHandler());
                var result = await client.GetString(new JsonHttpBody("foo"));
                Assert.AreEqual("foo", result);
            }            
        }

        [Proxy]
        interface IRawRequestBody
        {
            [Post("path")]
            Task<string> GetString(HttpBody request);
        }

        [Test]
        public async void RawResponseBody()
        {
            using (MockHttpServer.Json(x => x))
            {
                var client = HttpApiClient<IRawResponseBody>.Create("http://localhost:8844/path", new HttpClientHandler());
                var result = await client.GetString("foo");
                Assert.AreEqual("foo", (string)((JsonHttpBody)result).Json);
            }            
        }

        [Proxy]
        interface IRawResponseBody
        {
            [Post("path")]
            Task<HttpBody> GetString([Value]string value);
        }

        [Test]
        public async void ReflectString()
        {
            using (MockHttpServer.String(x => x))
            {
                var client = HttpApiClient<IReflectString>.Create("http://localhost:8844/path", new HttpClientHandler());
                var result = await client.ReflectString("foo");
                Assert.AreEqual("foo", result);
            }                        
        }

        [Proxy]
        interface IReflectString
        {
            [Post, Text]
            Task<string> ReflectString(string s);
        }
    }
}