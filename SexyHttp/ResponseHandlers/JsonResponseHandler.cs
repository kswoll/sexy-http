using System;
using System.Threading.Tasks;
using SexyHttp.HttpBodies;

namespace SexyHttp.ResponseHandlers
{
    public class JsonResponseHandler : HttpResponseHandler
    {
        public JsonResponseHandler() 
        {
        }

        public override Task<object> HandleResponse(HttpApiResponse response)
        {
            var jsonBody = response.Body as JsonHttpBody;
            if (jsonBody == null)
                throw new Exception("Expected a JsonHttpBody in the response");

            var result = jsonBody.Json.ToObject(ResponseType);
            return Task.FromResult(result);
        }
    }
}
