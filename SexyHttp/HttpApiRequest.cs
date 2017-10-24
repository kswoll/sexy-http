using System.Collections.Generic;
using System.Linq;
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

        public HttpApiRequest Clone()
        {
            var result = new HttpApiRequest
            {
                Method = Method,
                Url = Url.Clone(),
                Headers = Headers.ToList(),
                Body = Body,
                ResponseContentTypeOverride = ResponseContentTypeOverride
            };
            return result;
        }
    }
}