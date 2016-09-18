using System;
using System.Threading.Tasks;

namespace SexyHttp
{
    public interface IHttpApiInstrumenter
    {
        Task<HttpApiResponse> InstrumentCall(HttpApiRequest request, Func<HttpApiRequest, Task<HttpApiResponse>> inner);
    }
}