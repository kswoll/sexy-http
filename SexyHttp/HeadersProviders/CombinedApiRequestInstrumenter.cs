namespace SexyHttp.HeadersProviders
{
    public class CombinedApiRequestInstrumenter : IHttpApiRequestInstrumenter
    {
        private readonly IHttpApiRequestInstrumenter[] providers;

        public CombinedApiRequestInstrumenter(params IHttpApiRequestInstrumenter[] providers)
        {
            this.providers = providers;
        }

        public void InstrumentRequest(HttpApiRequest request)
        {
            foreach (var provider in providers)
            {
                provider.InstrumentRequest(request);
            }
        }
    }
}
