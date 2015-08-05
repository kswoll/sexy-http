using System.Threading.Tasks;

namespace SexyHttp
{
    public interface IHttpResponseHandler
    {
        Task<object> HandleResponse(HttpApiResponse response);
    }
}