using System.Threading.Tasks;

namespace SexyHttp.ResponseHandlers
{
    public class HttpApiResponseResponseHandler : HttpResponseHandler
    {
        public HttpApiResponseResponseHandler()
        {
            NonSuccessThrowsException = false;
        }

        protected override Task<object> ProvideResult(HttpApiResponse response)
        {
            return Task.FromResult<object>(response);
        }
    }
}
