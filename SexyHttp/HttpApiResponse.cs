using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SexyHttp
{
    public class HttpApiResponse
    {
        public HttpStatusCode StatusCode { get; }
        public IReadOnlyCollection<HttpHeader> Headers { get; }
        public HttpBody Body { get; }

        public HttpApiResponse(HttpStatusCode statusCode, IEnumerable<HttpHeader> headers, HttpBody body)
        {
            StatusCode = statusCode;
            Headers = headers.ToList();
            Body = body;
        }
    }
}