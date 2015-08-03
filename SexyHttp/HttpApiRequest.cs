using System.Collections.Generic;
using System.Net.Http;

namespace SexyHttp
{
    public class HttpApiRequest
    {
        public HttpMethod Method { get; set; }
        public string BaseUrl { get; set; }
        public HttpPath Path { get; set; }
        public List<HttpHeader> Headers { get; set; }
        public HttpBody Body { get; set; }
    }
}