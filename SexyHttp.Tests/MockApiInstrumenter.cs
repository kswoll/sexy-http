using System.Collections.Generic;
using System.Threading.Tasks;
using SexyHttp.Utils;

namespace SexyHttp.Tests
{
    public class MockApiInstrumenter : IHttpApiInstrumenter
    {
        public List<HttpHeader> Headers { get; } = new List<HttpHeader>();

        public Task InstrumentRequest(HttpApiRequest request)
        {
            request.Headers.AddRange(Headers);
            return TaskConstants.Completed;
        }

        public Task InstrumentResponse(HttpApiResponse response)
        {
            return TaskConstants.Completed;
        }
    }
}