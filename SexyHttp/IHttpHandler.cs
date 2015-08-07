using System;
using System.Threading.Tasks;

namespace SexyHttp
{
    public interface IHttpHandler
    {
        Task<T> Call<T>(HttpApiRequest request, Func<HttpApiResponse, Task<T>> responseHandler);
    }
}