using SexyHttp.TypeConverters;

namespace SexyHttp
{
    public interface IHttpHeadersProvider
    {
        void ProvideHeaders(HttpApiRequest request);
    }
}