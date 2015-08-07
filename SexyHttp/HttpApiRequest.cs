using System.Collections.Generic;
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
    }
}