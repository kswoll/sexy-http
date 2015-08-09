using System.Threading.Tasks;
using SexyHttp.HttpBodies;
using SexyHttp.TypeConverters;

namespace SexyHttp.ArgumentHandlers
{
    public class RawArgumentHandler : HttpArgumentHandler
    {
        public RawArgumentHandler(ITypeConverter typeConverter) : base(typeConverter)
        {
        }

        public override Task ApplyArgument(HttpApiRequest request, string name, object argument)
        {
            request.Body = new StringHttpBody((string)argument);

            return base.ApplyArgument(request, name, argument);
        }
    }
}
