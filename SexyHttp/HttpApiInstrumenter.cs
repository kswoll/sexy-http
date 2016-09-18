using System;
using System.Threading.Tasks;

namespace SexyHttp
{
    public delegate Task<HttpApiResponse> HttpApiInstrumenter(HttpApiRequest request, Func<HttpApiRequest, Task<HttpApiResponse>> inner);
}