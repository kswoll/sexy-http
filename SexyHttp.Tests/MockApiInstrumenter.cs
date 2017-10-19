using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SexyHttp.Tests
{
    public class MockApiInstrumenter : IHttpApiInstrumenter
    {
        public List<HttpHeader> Headers { get; } = new List<HttpHeader>();

        public async Task<HttpHandlerResponse> InstrumentCall(HttpApiEndpoint endpoint, HttpApiRequest request, Func<HttpApiRequest, Task<HttpHandlerResponse>> inner)
        {
            request.Headers.AddRange(Headers);
            return await inner(request);
        }
    }
}