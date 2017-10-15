using System;
using System.Linq;
using System.Threading.Tasks;

namespace SexyHttp.Instrumenters
{
    public class CombinedInstrumenter : IHttpApiInstrumenter
    {
        private readonly HttpApiInstrumenter[] instrumenters;

        public CombinedInstrumenter(params HttpApiInstrumenter[] instrumenters)
        {
            this.instrumenters = instrumenters;
        }

        public async Task<HttpApiResponse> InstrumentCall(HttpApiEndpoint endpoint, HttpApiRequest request, Func<HttpApiRequest, Task<HttpApiResponse>> inner)
        {
            if (instrumenters.Length == 0)
                return await inner(request);

            var current = inner;
            foreach (var instrumenter in instrumenters.Skip(1).Reverse())
            {
                current = apiRequest => instrumenter(endpoint, apiRequest, current);
            }

            return await instrumenters.First()(endpoint, request, current);
        }
    }
}
