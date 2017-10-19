using System;
using System.Threading.Tasks;

namespace SexyHttp
{
    public interface IHttpApiInstrumenter
    {
        Task<HttpHandlerResponse> InstrumentCall(HttpApiEndpoint endpoint, HttpApiRequest request, Func<HttpApiRequest, Task<HttpHandlerResponse>> inner);
    }
}