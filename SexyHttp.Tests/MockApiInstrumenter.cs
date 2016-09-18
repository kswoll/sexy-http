using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SexyHttp.Tests
{
    public class MockApiInstrumenter : IHttpApiInstrumenter
    {
        public List<HttpHeader> Headers { get; } = new List<HttpHeader>();

        public async Task<HttpApiResponse> InstrumentCall(HttpApiRequest request, Func<HttpApiRequest, Task<HttpApiResponse>> inner)
        {
            request.Headers.AddRange(Headers);
            return await inner(request);
        }
    }
}