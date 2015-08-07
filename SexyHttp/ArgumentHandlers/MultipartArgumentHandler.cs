using System;
using System.Threading.Tasks;
using SexyHttp.HttpBodies;
using SexyHttp.TypeConverters;
using SexyHttp.Utils;

namespace SexyHttp.ArgumentHandlers
{
    public class MultipartArgumentHandler : HttpArgumentHandler
    {
        public MultipartArgumentHandler(ITypeConverter typeConverter) : base(typeConverter)
        {
        }

        public override Task ApplyArgument(HttpApiRequest request, string name, object argument)
        {
            if (request.Body == null)
            {
                request.Body = new MultipartHttpBody();
            }
            var multipart = (MultipartHttpBody)request.Body;
            var body = TypeConverter.ConvertTo<HttpBody>(argument);
            if (body == null)
                throw new Exception($"Could not create body for {name}");
            multipart.Data[name] = new MultipartData { Body = body };

            return TaskConstants.Completed;
        }
    }
}
