using System;
using System.Threading.Tasks;

namespace SexyHttp
{
    public interface IHttpApiInstrumenter
    {
        Task<HttpApiResponse> InstrumentCall(HttpApiEndpoint endpoint, HttpApiRequest request, Func<HttpApiRequest, Task<HttpApiResponse>> inner);
    }
}