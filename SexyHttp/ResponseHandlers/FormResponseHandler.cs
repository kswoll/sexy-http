using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SexyHttp.HttpBodies;

namespace SexyHttp.ResponseHandlers
{
    public class FormResponseHandler : HttpResponseHandler
    {
        public override Task<object> HandleResponse(HttpApiResponse response)
        {
            // Use JSON.NET to do the deserialization 
            var form = (FormHttpBody)response.Body;
            var json = JToken.FromObject(form.Values);
            return Task.FromResult(json.ToObject(ResponseType));
        }
    }
}
