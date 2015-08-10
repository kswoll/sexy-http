using System.Threading.Tasks;
using SexyHttp.HttpBodies;
using SexyHttp.TypeConverters;

namespace SexyHttp.ArgumentHandlers
{
    public class StringArgumentHandler : HttpArgumentHandler
    {
        public StringArgumentHandler(ITypeConverter typeConverter) : base(typeConverter)
        {
        }

        public override Task ApplyArgument(HttpApiRequest request, string name, object argument)
        {
            request.Body = new StringHttpBody((string)argument);

            return base.ApplyArgument(request, name, argument);
        }
    }
}
