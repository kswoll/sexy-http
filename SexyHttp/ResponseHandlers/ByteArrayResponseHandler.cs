using System.Threading.Tasks;
using SexyHttp.HttpBodies;

namespace SexyHttp.ResponseHandlers
{
    public class ByteArrayResponseHandler : HttpResponseHandler
    {
        public override Task<object> HandleResponse(HttpApiResponse response)
        {
            return Task.FromResult<object>(((ByteArrayHttpBody)response.Body).Data);
        }
    }
}
