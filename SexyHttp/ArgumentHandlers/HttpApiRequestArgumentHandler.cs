using System;
using System.Threading.Tasks;
using SexyHttp.TypeConverters;

namespace SexyHttp.ArgumentHandlers
{
    public class HttpApiRequestArgumentHandler : HttpArgumentHandler
    {
        public HttpApiRequestArgumentHandler(ITypeConverter typeConverter) : base(typeConverter)
        {
        }

        public override Task ApplyArgument(HttpApiRequest request, string name, object argument)
        {
            var requestHandler = (Action<HttpApiRequest>)argument;
            requestHandler(request);

            return base.ApplyArgument(request, name, argument);
        }
    }
}