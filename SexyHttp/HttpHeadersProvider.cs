using SexyHttp.TypeConverters;

namespace SexyHttp
{
    public abstract class HttpHeadersProvider : IHttpHeadersProvider
    {
        public abstract void ProvideHeaders(HttpApiRequest request);

        public ITypeConverter TypeConverter { get; }

        protected HttpHeadersProvider(ITypeConverter typeConverter)
        {
            TypeConverter = typeConverter;
        }
    }
}
