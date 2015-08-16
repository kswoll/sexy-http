using System;
using System.Threading.Tasks;
using SexyHttp.HttpBodies;
using SexyHttp.TypeConverters;

namespace SexyHttp.ResponseHandlers
{
    public class JsonResponseHandler : HttpResponseHandler
    {
        protected override Task<object> ProvideResult(HttpApiRequest request, HttpApiResponse response)
        {
            var jsonBody = response.Body as JsonHttpBody;
            if (jsonBody == null)
                throw new Exception("Expected a JsonHttpBody in the response");

            var result = TypeConverter.ConvertTo(TypeConversionContext.Body, ResponseType, jsonBody.Json);
            return Task.FromResult(result);
        }
    }
}
