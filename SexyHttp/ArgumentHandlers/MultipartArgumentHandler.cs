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
            multipart.Data[name] = new MultipartData { Body = new StringHttpBody((string)argument) };

            return TaskConstants.Completed;
        }
    }
}
