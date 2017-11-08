using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SexyHttp.HttpBodies;
using SexyHttp.Tests.Utils;

namespace SexyHttp.HttpHandlers
{
    /// <summary>
    /// Use this implementation whenever you find yourself in a situation where you cannot use
    /// async semantics.  (For example, a constructor)  If you use HttpClientHttpHandler, you
    /// will likely find yourself in a situation where you have to deal with deadlocks.
    /// </summary>
    public class WebRequestHttpHandler : IHttpHandler
    {
        public Task<HttpHandlerResponse> Call(HttpApiRequest request)
        {
            var webRequest = WebRequest.CreateHttp(request.Url.ToString());
            webRequest.Method = request.Method.ToString();
            foreach (var header in request.Headers)
            {
                switch (header.Name)
                {
                    case "User-Agent":
                        webRequest.UserAgent = header.Values.Single();
                        break;
                    case "Accept":
                        webRequest.Accept = header.Values.Single();
                        break;
                    default:
                        webRequest.Headers.Add(header.Name, string.Join(",", header.Values));
                        break;
                }
            }

            var requestWriteTime = new Stopwatch();
            requestWriteTime.Start();
            if (request.Body != null)
            {
                var requestStream = webRequest.GetRequestStream();
                var stream = request.Body.Accept(new ContentCreator(webRequest));
                stream.CopyTo(requestStream);
                requestStream.Close();
            }
            else if (request.Method != HttpMethod.Get && request.Method != HttpMethod.Head)
            {
                var requestStream = webRequest.GetRequestStream();
                requestStream.Close();
            }

            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)webRequest.GetResponse();
            }
            catch (WebException e) when (e.Response != null)
            {
                response = (HttpWebResponse)e.Response;
            }

            requestWriteTime.Stop();

            var responseHeaders = new List<HttpHeader>();
            HttpBody responseBody = null;

            responseHeaders.AddRange(response.Headers.AllKeys.Select(x => new HttpHeader(x, response.Headers[x].Split(','))));

            var responseReadTime = new Stopwatch();
            responseReadTime.Start();

            var responseStream = response.GetResponseStream();

            switch (request.ResponseContentTypeOverride ?? response.ContentType.Split(';')[0])
            {
                case "application/json":
                    var json = JToken.Parse(Encoding.UTF8.GetString(responseStream.ReadToEnd()));
                    responseBody = new JsonHttpBody(json);
                    break;
                case "application/x-www-form-urlencoded":
                    throw new NotSupportedException();
//                    var stream = await message.Content.ReadAsStreamAsync();
//                    body = FormParser.ParseForm(stream);
//                    break;
                case "text/plain":
                case "":
                    var text = Encoding.UTF8.GetString(responseStream.ReadToEnd());
                    responseBody = new StringHttpBody(text);
                    break;
                case "application/octet-stream":
                    var stream = new MemoryStream();
                    responseStream.CopyTo(stream);
                    stream.Position = 0;
                    responseBody = new StreamHttpBody(stream);
                    break;
            }

            responseStream.Close();
            responseReadTime.Stop();

            var result = new HttpApiResponse(response.StatusCode, responseBody, responseHeaders, response.ResponseUri.ToString());
            return Task.FromResult(new HttpHandlerResponse(result, requestWriteTime.Elapsed, responseReadTime.Elapsed));
        }

        private class ContentCreator : IHttpBodyVisitor<Stream>
        {
            private readonly HttpWebRequest request;

            public ContentCreator(HttpWebRequest request)
            {
                this.request = request;
            }

            public Stream VisitJsonBody(JsonHttpBody body)
            {
                request.ContentType = "application/json";

                var text = body.Json.ToString(Formatting.Indented);
                var result = new MemoryStream(Encoding.UTF8.GetBytes(text));
                result.Position = 0;
                return result;
            }

            public Stream VisitStringBody(StringHttpBody body)
            {
                request.ContentType = "text/plain";

                var result = new MemoryStream(Encoding.UTF8.GetBytes(body.Text));
                result.Position = 0;
                return result;
            }

            public Stream VisitMultipartBody(MultipartHttpBody body)
            {
                throw new NotSupportedException();
/*
                var content = new MultipartFormDataContent();
                foreach (var item in body.Data)
                {
                    var itemContent = item.Value.Body.Accept(this);
                    content.Add(itemContent, item.Key);
                }
                return content;
*/
            }

            public Stream VisitByteArrayBody(ByteArrayHttpBody body)
            {
                request.ContentType = "application/octet-stream";
                var result = new MemoryStream(body.Data);
                result.Position = 0;
                return result;
            }

            public Stream VisitStreamBody(StreamHttpBody body)
            {
                request.ContentType = "application/octet-stream";
                var result = body.Stream;
                return result;
            }

            public Stream VisitFormBody(FormHttpBody body)
            {
                throw new NotSupportedException();
/*
                var result = new FormUrlEncodedContent(body.Values);
                return result;
*/
            }
        }
    }
}
