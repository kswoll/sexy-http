namespace SexyHttp.HeadersProviders
{
    public class CombinedHeadersProvider : IHttpHeadersProvider
    {
        private readonly IHttpHeadersProvider[] providers;

        public CombinedHeadersProvider(params IHttpHeadersProvider[] providers)
        {
            this.providers = providers;
        }

        public void ProvideHeaders(HttpApiRequest request)
        {
            foreach (var provider in providers)
            {
                provider.ProvideHeaders(request);
            }
        }
    }
}
