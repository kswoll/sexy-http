using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SexyHttp.RequestInstrumenters
{
    public class CombinedRequestInstrumenter : IHttpApiRequestInstrumenter
    {
        private readonly IHttpApiRequestInstrumenter[] instrumenters;

        public CombinedRequestInstrumenter(params IHttpApiRequestInstrumenter[] instrumenters)
        {
            this.instrumenters = instrumenters;
        }

        public async Task InstrumentRequest(HttpApiRequest request)
        {
            foreach (var instrumenter in instrumenters)
            {
                await instrumenter.InstrumentRequest(request);
            }
        }
    }
}
