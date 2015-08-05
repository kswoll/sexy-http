using System.Collections.Generic;

namespace SexyHttp.Tests
{
    public class MockApiRequestInstrumenter : IHttpApiRequestInstrumenter
    {
        public List<HttpHeader> Headers { get; } = new List<HttpHeader>();

        public void InstrumentRequest(HttpApiRequest request)
        {
            request.Headers.AddRange(Headers);
        }
    }
}