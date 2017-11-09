using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using SexyHttp.Urls;

namespace SexyHttp
{
    public class HttpApiRequest
    {
        public HttpMethod Method { get; set; }
        public HttpUrl Url { get; set; }
        public List<HttpHeader> Headers { get; set; }
        public HttpBody Body { get; set; }
        public string ResponseContentTypeOverride { get; set; }
        public IWebProxy Proxy { get; set; }

        /// <summary>
        /// You may use this dictionary to store custom values useful when instrumenting an
        /// API call and you need to track state around a particular request.
        /// </summary>
        public Dictionary<object, object> Items { get; set; } = new Dictionary<object, object>();

        public HttpApiRequest Clone()
        {
            var result = new HttpApiRequest
            {
                Method = Method,
                Url = Url.Clone(),
                Headers = Headers.ToList(),
                Items = Items.ToDictionary(x => x.Key, x => x.Value),
                Body = Body,
                ResponseContentTypeOverride = ResponseContentTypeOverride
            };
            return result;
        }
    }
}