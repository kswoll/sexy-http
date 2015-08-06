using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SexyHttp.HttpBodies;

namespace SexyHttp.HttpHandlers.HttpClientHandlers
{
    public class HttpClientHandler : IHttpHandler
    {
        public async Task<HttpApiResponse> Call(HttpApiRequest request)
        {
            using (var client = new HttpClient())
            {
                var response = await client.SendAsync(CreateRequestMessage(request));
                var result = await CreateResponse(response);
                return result;
            }
        }

        private HttpRequestMessage CreateRequestMessage(HttpApiRequest request)
        {
            var message = new HttpRequestMessage(request.Method, request.Url.ToString());
            foreach (var header in request.Headers)
            {
                message.Headers.Add(header.Name, header.Values);
            }
            if (request.Body != null)
            {
                message.Content = request.Body.Accept(new ContentCreator());
            }
            return message;
        }

        private class ContentCreator : IHttpBodyVisitor<HttpContent>
        {
            public HttpContent VisitJsonBody(JsonHttpBody body)
            {
                var text = body.Json.ToString(Formatting.Indented);
                var content = new StringContent(text);
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                return content;
            }

            public HttpContent VisitStringBody(StringHttpBody body)
            {
                var content = new StringContent(body.Text);
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");
                return content;
            }
        }

        private async Task<HttpApiResponse> CreateResponse(HttpResponseMessage message)
        {
            HttpBody body = null;
            if (message.Content != null)
            {
                switch (message.Content.Headers.ContentType.MediaType)
                {
                    case "application/json":
                        var json = JToken.Parse(await message.Content.ReadAsStringAsync());
                        body = new JsonHttpBody(json);
                        break;
                    case "text/plain":
                        var text = await message.Content.ReadAsStringAsync();
                        body = new StringHttpBody(text);
                        break;
                }
            }

            var response = new HttpApiResponse(message.StatusCode, body, message.Headers.Select(x => new HttpHeader(x.Key, x.Value.ToArray())));
            return response;
        }
    }
}
