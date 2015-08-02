using System.Collections.Generic;

namespace SexyHttp
{
    public class HttpApiRequest
    {
        public string Method { get; set; }
        public string Url { get; set; }
        public List<HttpHeader> Headers { get; set; }
        public HttpBody Body { get; set; }
    }
}