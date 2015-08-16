using System.Threading.Tasks;
using SexyHttp.TypeConverters;

namespace SexyHttp
{
    public abstract class HttpApiRequestInstrumenter : IHttpApiRequestInstrumenter
    {
        public abstract Task InstrumentRequest(HttpApiRequest request);

        public ITypeConverter TypeConverter { get; }

        protected HttpApiRequestInstrumenter(ITypeConverter typeConverter)
        {
            TypeConverter = typeConverter;
        }
    }
}
