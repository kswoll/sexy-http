using System.Threading.Tasks;

namespace SexyHttp
{
    public interface IHttpHandler
    {
        Task<HttpHandlerResponse> Call(HttpApiRequest request);
    }
}