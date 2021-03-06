﻿using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SexyHttp
{
    public class HttpApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public List<HttpHeader> Headers { get; }
        public HttpBody Body { get; set; }
        public string ResponseUri { get; set; }

        public HttpApiResponse(HttpStatusCode statusCode = HttpStatusCode.OK, HttpBody body = null, IEnumerable<HttpHeader> headers = null, string responseUri = null)
        {
            var headersList = headers?.ToList() ?? new List<HttpHeader>();

            StatusCode = statusCode;
            ResponseUri = responseUri;
            Headers = headersList;
            Body = body;
        }
    }
}