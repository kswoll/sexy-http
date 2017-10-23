using System.Linq;

namespace SexyHttp.Instrumenters
{
    public class CombinedInstrumenter : IHttpApiInstrumenter
    {
        private readonly HttpApiInstrumenter[] instrumenters;

        public CombinedInstrumenter(params HttpApiInstrumenter[] instrumenters)
        {
            this.instrumenters = instrumenters;
        }

        public IHttpApiInstrumentation InstrumentCall(HttpApiEndpoint endpoint, HttpApiArguments arguments, IHttpApiInstrumentation inner)
        {
            var instrumentation = inner;
            foreach (var instrumenter in instrumenters.Reverse())
                instrumentation = instrumenter(endpoint, arguments, instrumentation);
            return instrumentation;
        }
    }
}
