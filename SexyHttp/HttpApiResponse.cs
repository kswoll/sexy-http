using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SexyHttp
{
    public class HttpApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public List<HttpHeader> Headers { get; }
        public HttpBody Body { get; set; }

        public HttpApiResponse(HttpStatusCode statusCode = HttpStatusCode.OK, HttpBody body = null, IEnumerable<HttpHeader> headers = null)
        {
            var headersList = headers?.ToList() ?? new List<HttpHeader>();

            StatusCode = statusCode;
            Headers = headersList;
            Body = body;
        }
    }
}