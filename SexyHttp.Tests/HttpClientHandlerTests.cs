using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SexyHttp.HttpBodies;
using SexyHttp.HttpHandlers.HttpClientHandlers;
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
            using (MockHttpServer.ReturnJson(request => new JValue("foo")))
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
            Task<string> ReflectValue(string argument);
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
    }
}