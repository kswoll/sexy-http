using System.Threading.Tasks;

namespace SexyHttp.Instrumenters
{
    public class CombinedInstrumenter : IHttpApiInstrumenter
    {
        private readonly IHttpApiInstrumenter[] instrumenters;

        public CombinedInstrumenter(params IHttpApiInstrumenter[] instrumenters)
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

        public async Task InstrumentResponse(HttpApiResponse response)
        {
            foreach (var instrumenter in instrumenters)
            {
                await instrumenter.InstrumentResponse(response);
            }
        }
    }
}
