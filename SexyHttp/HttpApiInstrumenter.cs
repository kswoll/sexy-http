using System.Threading.Tasks;
using SexyHttp.TypeConverters;

namespace SexyHttp
{
    public abstract class HttpApiInstrumenter : IHttpApiInstrumenter
    {
        public abstract Task InstrumentRequest(HttpApiRequest request);
        public abstract Task InstrumentResponse(HttpApiResponse response);

        public ITypeConverter TypeConverter { get; }

        protected HttpApiInstrumenter(ITypeConverter typeConverter)
        {
            TypeConverter = typeConverter;
        }
    }
}
