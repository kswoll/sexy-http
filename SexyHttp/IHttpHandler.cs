using System.Threading.Tasks;

namespace SexyHttp
{
    public interface IHttpHandler
    {
        Task<HttpApiResponse> Call(HttpApiRequest request);
    }
}