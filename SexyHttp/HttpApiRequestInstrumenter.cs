using SexyHttp.TypeConverters;

namespace SexyHttp
{
    public abstract class HttpApiRequestInstrumenter : IHttpApiRequestInstrumenter
    {
        public abstract void InstrumentRequest(HttpApiRequest request);

        public ITypeConverter TypeConverter { get; }

        protected HttpApiRequestInstrumenter(ITypeConverter typeConverter)
        {
            TypeConverter = typeConverter;
        }
    }
}
