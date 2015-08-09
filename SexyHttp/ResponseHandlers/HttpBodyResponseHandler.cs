using System.Threading.Tasks;

namespace SexyHttp.ResponseHandlers
{
    public class HttpBodyResponseHandler : HttpResponseHandler
    {
        public override Task<object> HandleResponse(HttpApiResponse response)
        {
            return Task.FromResult<object>(response.Body);
        }
    }
}
