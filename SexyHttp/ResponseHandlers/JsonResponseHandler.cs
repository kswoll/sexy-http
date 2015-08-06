using System;
using System.Threading.Tasks;
using SexyHttp.HttpBodies;

namespace SexyHttp.ResponseHandlers
{
    public class JsonResponseHandler : HttpResponseHandler
    {
        public Type Type { get; }

        public JsonResponseHandler(Type type) 
        {
            Type = type;
        }

        public override Task<object> HandleResponse(HttpApiResponse response)
        {
            var jsonBody = response.Body as JsonHttpBody;
            if (jsonBody == null)
                throw new Exception("Expected a JsonHttpBody in the response");

            var result = jsonBody.Json.ToObject(Type);
            return Task.FromResult(result);
        }
    }
}
