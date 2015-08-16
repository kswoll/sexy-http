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

        public HttpApiResponse(HttpStatusCode statusCode = HttpStatusCode.OK, HttpBody body = null, IEnumerable<HttpHeader> headers = null)
        {
            var headersList = headers?.ToList() ?? new List<HttpHeader>();

            StatusCode = statusCode;
            Headers = headersList;
            Body = body;
        }
    }
}