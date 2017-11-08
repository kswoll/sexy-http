using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SexyHttp.HttpBodies;
using System.Diagnostics;

namespace SexyHttp.HttpHandlers
{
    /// <summary>
    /// Uses an instance of HttpClient to perform all the HTTP interactions.  Use this if you are
    /// confident you are able to use "async all the way".  If you are in a situation where you
    /// will ultimately have to interact with the API in a non-async context, you are much better
    /// off using WebRequestHttpHandler, as this will avoid deadlocks.
    /// </summary>
    public class HttpClientHttpHandler : IHttpHandler
    {
        private readonly Func<HttpClientHandler> handler;

        public HttpClientHttpHandler(Func<HttpClientHandler> handler = null)
        {
            this.handler = handler ?? (() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            });
        }

        public async Task<HttpHandlerResponse> Call(HttpApiRequest request)
        {
            using (var client = new HttpClient(handler()))
            {
                try
                {
                    var requestWriteTime = new Stopwatch();
                    requestWriteTime.Start();

                    var response = await client.SendAsync(CreateRequestMessage(request));
                    requestWriteTime.Stop();

                    var responseReadTime = new Stopwatch();
                    responseReadTime.Start();
                    var result = await CreateResponse(request, response);
                    responseReadTime.Stop();

                    return new HttpHandlerResponse(result, requestWriteTime.Elapsed, responseReadTime.Elapsed);
                }
                catch (HttpRequestException e) when (IsProxyRequiredException(e, out var response))
                {
                    return new HttpHandlerResponse(response, TimeSpan.Zero, TimeSpan.Zero);
                }
                catch (Exception e)
                {
                    throw new Exception($"Error making {request.Method} request to {request.Url}", e);
                }
            }
        }

        private static bool IsProxyRequiredException(HttpRequestException e, out HttpApiResponse apiResponse)
        {
            if (e.InnerException is WebException webException && webException.Response is HttpWebResponse response &&
                response.StatusCode == HttpStatusCode.ProxyAuthenticationRequired)
            {
                var responseHeaders = new List<HttpHeader>();
                responseHeaders.AddRange(response.Headers.AllKeys.Select(x => new HttpHeader(x, response.Headers[x].Split(','))));
                apiResponse = new HttpApiResponse(HttpStatusCode.ProxyAuthenticationRequired, headers: responseHeaders, responseUri: response.ResponseUri.ToString());
                return true;
            }
            else
            {
                apiResponse = null;
                return false;
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

            public HttpContent VisitMultipartBody(MultipartHttpBody body)
            {
                var content = new MultipartFormDataContent();
                foreach (var item in body.Data)
                {
                    var itemContent = item.Value.Body.Accept(this);
                    content.Add(itemContent, item.Key);
                }
                return content;
            }

            public HttpContent VisitByteArrayBody(ByteArrayHttpBody body)
            {
                var result = new ByteArrayContent(body.Data);
                result.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                return result;
            }

            public HttpContent VisitStreamBody(StreamHttpBody body)
            {
                var result = new StreamContent(body.Stream);
                result.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                return result;
            }

            public HttpContent VisitFormBody(FormHttpBody body)
            {
                var result = new FormUrlEncodedContent(body.Values);
                return result;
            }
        }

        private async Task<HttpApiResponse> CreateResponse(HttpApiRequest request, HttpResponseMessage message)
        {
            HttpBody body = null;
            var headers = new List<HttpHeader>();
            if (message.Content != null)
            {
                headers.AddRange(message.Content.Headers.Select(x => new HttpHeader(x.Key, x.Value.ToArray())));
                switch (request.ResponseContentTypeOverride ?? message.Content.Headers.ContentType?.MediaType)
                {
                    case "application/json":
                        var json = JToken.Parse(await message.Content.ReadAsStringAsync());
                        body = new JsonHttpBody(json);
                        break;
                    case "application/x-www-form-urlencoded":
                        var stream = await message.Content.ReadAsStreamAsync();
                        body = FormParser.ParseForm(stream);
                        break;
                    case "text/plain":
                        var text = await message.Content.ReadAsStringAsync();
                        body = new StringHttpBody(text);
                        break;
                    case "application/octet-stream":
                        body = new StreamHttpBody(await message.Content.ReadAsStreamAsync());
                        break;
                    case null:
                        break;
                }
            }
            foreach (var header in message.Headers)
            {
                if (!headers.Any(x => x.Name == header.Key))
                    headers.Add(new HttpHeader(header.Key, header.Value.ToArray()));
            }

            var response = new HttpApiResponse(message.StatusCode, body, headers);
            return response;
        }
    }
}
