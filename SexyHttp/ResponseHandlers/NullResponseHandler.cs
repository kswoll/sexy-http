using System.Threading.Tasks;

namespace SexyHttp.ResponseHandlers
{
    public class NullResponseHandler : IHttpResponseHandler
    {
        public Task<object> HandleResponse(HttpApiResponse response)
        {
            return Task.FromResult<object>(null);
        }
    }
}
