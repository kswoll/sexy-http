using System.Threading.Tasks;
using SexyHttp.HttpBodies;

namespace SexyHttp.ResponseHandlers
{
    public class StringResponseHandler : HttpResponseHandler
    {
        public override Task<object> HandleResponse(HttpApiResponse response)
        {
            return Task.FromResult<object>(((StringHttpBody)response.Body).Text);
        }
    }
}
