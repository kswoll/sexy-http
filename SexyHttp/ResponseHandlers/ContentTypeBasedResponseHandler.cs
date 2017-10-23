using System;
using System.Linq;
using System.Threading.Tasks;
using SexyHttp.TypeConverters;

namespace SexyHttp.ResponseHandlers
{
    public class ContentTypeBasedResponseHandler : HttpResponseHandler
    {
        private readonly JsonResponseHandler jsonHandler;
        private readonly FormResponseHandler formHandler;
        private readonly StringResponseHandler stringHandler;

        public ContentTypeBasedResponseHandler()
        {
            jsonHandler = new JsonResponseHandler();
            formHandler = new FormResponseHandler();
            stringHandler = new StringResponseHandler();
        }

        protected override Task<object> ProvideResult(HttpApiRequest request, HttpApiResponse response)
        {
            var contentType = response.Headers.SingleOrDefault(x => x.Name == "Content-Type")?.Values?.Single();
            contentType = contentType ?? "application/json";

            switch (contentType)
            {
                case "application/x-www-form-urlencoded":
                    return formHandler.HandleResponse(request, response);
                case "text/plain":
                    return stringHandler.HandleResponse(request, response);
                default:
                    return jsonHandler.HandleResponse(request, response);
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
