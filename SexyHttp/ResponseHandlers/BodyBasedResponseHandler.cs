using System;
using System.Threading.Tasks;
using SexyHttp.HttpBodies;
using SexyHttp.TypeConverters;

namespace SexyHttp.ResponseHandlers
{
    public class BodyBasedResponseHandler : HttpResponseHandler
    {
        private readonly JsonResponseHandler jsonHandler;
        private readonly FormResponseHandler formHandler;
        private readonly StringResponseHandler stringHandler;

        public BodyBasedResponseHandler()
        {
            jsonHandler = new JsonResponseHandler();
            formHandler = new FormResponseHandler();
            stringHandler = new StringResponseHandler();
        }

        protected override Task<object> ProvideResult(HttpApiRequest request, HttpApiResponse response)
        {
            switch (response.Body)
            {
                case JsonHttpBody _:
                    return jsonHandler.HandleResponse(request, response);
                case StringHttpBody _:
                    return stringHandler.HandleResponse(request, response);
                case FormHttpBody _:
                    return formHandler.HandleResponse(request, response);
                default:
                    return Task.FromResult<object>(null);
            }
        }

        public override ITypeConverter TypeConverter
        {
            get => base.TypeConverter;
            set
            {
                base.TypeConverter = value;
                formHandler.TypeConverter = value;
                jsonHandler.TypeConverter = value;
                stringHandler.TypeConverter = value;
            }
        }

        public override Type ResponseType
        {
            get => base.ResponseType;
            set
            {
                base.ResponseType = value;
                formHandler.ResponseType = value;
                jsonHandler.ResponseType = value;
                stringHandler.ResponseType = value;
            }
        }
    }
}
