using System.Collections.Generic;
using System.Threading.Tasks;
using SexyHttp.Utils;

namespace SexyHttp.Tests
{
    public class MockApiRequestInstrumenter : IHttpApiRequestInstrumenter
    {
        public List<HttpHeader> Headers { get; } = new List<HttpHeader>();

        public Task InstrumentRequest(HttpApiRequest request)
        {
            request.Headers.AddRange(Headers);
            return TaskConstants.Completed;
        }
    }
}