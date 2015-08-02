using System.Collections.Generic;

namespace SexyHttp.Tests
{
    public class MockHeadersProvider : IHttpHeadersProvider
    {
        public List<HttpHeader> Headers { get; } = new List<HttpHeader>();

        public void ProvideHeaders(HttpApiRequest request)
        {
            request.Headers.AddRange(Headers);
        }
    }
}