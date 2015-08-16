using System.Threading.Tasks;

namespace SexyHttp.ResponseHandlers
{
    public class NullResponseHandler : HttpResponseHandler
    {
        protected override Task<object> ProvideResult(HttpApiResponse response)
        {
            return Task.FromResult<object>(null);
        }
    }
}
