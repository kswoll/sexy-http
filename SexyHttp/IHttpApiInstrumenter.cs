using System.Threading.Tasks;

namespace SexyHttp
{
    public interface IHttpApiInstrumenter
    {
        Task InstrumentRequest(HttpApiRequest request);
        Task InstrumentResponse(HttpApiResponse response);
    }
}