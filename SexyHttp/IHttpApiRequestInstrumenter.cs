using System.Threading.Tasks;

namespace SexyHttp
{
    public interface IHttpApiRequestInstrumenter
    {
        Task InstrumentRequest(HttpApiRequest request);
    }
}