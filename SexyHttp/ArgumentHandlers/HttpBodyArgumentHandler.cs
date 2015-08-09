using System.Threading.Tasks;
using SexyHttp.TypeConverters;

namespace SexyHttp.ArgumentHandlers
{
    public class HttpBodyArgumentHandler: HttpArgumentHandler
    {
        public HttpBodyArgumentHandler(ITypeConverter typeConverter) : base(typeConverter)
        {
        }

        public override Task ApplyArgument(HttpApiRequest request, string name, object argument)
        {
            request.Body = (HttpBody)argument;

            return base.ApplyArgument(request, name, argument);
        }
    }
}
