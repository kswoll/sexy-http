namespace SexyHttp
{
    public interface IHttpHeadersProvider
    {
        void ProvideHeaders(HttpApiRequest request);
    }
}