using System.Collections.Generic;

namespace SexyHttp.Tests
{
    public class MockApiInstrumenter : IHttpApiInstrumenter
    {
        public List<HttpHeader> Headers { get; } = new List<HttpHeader>();

        public IHttpApiInstrumentation InstrumentCall(HttpApiEndpoint endpoint, HttpApiArguments arguments, IHttpApiInstrumentation inner)
        {
            return new HttpApiInstrumentation(inner, () =>
            {
                var request = inner.GetRequest();
                request.Headers.AddRange(Headers);
                return request;
            });
        }
    }
}