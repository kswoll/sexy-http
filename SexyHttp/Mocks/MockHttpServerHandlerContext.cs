using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SexyHttp.HttpBodies;
using SexyHttp.Tests.Utils;

namespace SexyHttp.Mocks
{
    public class MockHttpServerHandlerContext
    {
        public HttpListenerRequest Request { get; }
        public HttpListenerResponse Response { get; set; }

        public MockHttpServerHandlerContext(HttpListenerRequest request, HttpListenerResponse response)
        {
            Request = request;
            Response = response;
        }

        public async Task WriteJson(JToken json)
        {
            Response.Headers.Add("Content-Type", "application/json");
            var s = json.ToString(Formatting.Indented);
            var buffer = Encoding.UTF8.GetBytes(s);
            await Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        public async Task WriteByteArray(byte[] data)
        {
            Response.Headers.Add("Content-Type", "application/octet-stream");
            await Response.OutputStream.WriteAsync(data, 0, data.Length);
        }

        public async Task WriteForm(FormHttpBody form)
        {
            Response.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            var content = new FormUrlEncodedContent(form.Values);
            var data = await content.ReadAsStreamAsync();
            await data.CopyToAsync(Response.OutputStream);
        }

        public async Task WriteString(string s)
        {
            Response.Headers.Add("Content-Type", "text/plain");
            var buffer = Encoding.UTF8.GetBytes(s);
            await Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        public async Task<JToken> ReadJson()
        {
            using (var reader = new StreamReader(Request.InputStream))
            {
                var inputString = await reader.ReadToEndAsync();
                var jsonInput = JToken.Parse(inputString);
                return jsonInput;
            }
        }

        public async Task<string> ReadString()
        {
            using (var reader = new StreamReader(Request.InputStream))
            {
                var inputString = await reader.ReadToEndAsync();
                return inputString;
            }
        }

        public MultipartHttpBody ReadMultipart()
        {
            return MultipartParser.ParseMultipart(Request);
        }

        public FormHttpBody ReadForm()
        {
            return FormParser.ParseForm(Request.InputStream);
        }

        public async Task<byte[]> ReadByteArray()
        {
            return await Request.InputStream.ReadToEndAsync();
        }
    }
}
