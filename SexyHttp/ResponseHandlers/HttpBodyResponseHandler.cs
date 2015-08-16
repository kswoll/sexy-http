using System.Threading.Tasks;

namespace SexyHttp.ResponseHandlers
{
    public class HttpBodyResponseHandler : HttpResponseHandler
    {
        protected override Task<object> ProvideResult(HttpApiRequest request, HttpApiResponse response)
        {
            return Task.FromResult<object>(response.Body);
        }
    }
}
