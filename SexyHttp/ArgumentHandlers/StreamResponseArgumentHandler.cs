using System;
using System.IO;
using System.Threading.Tasks;
using SexyHttp.HttpBodies;
using SexyHttp.TypeConverters;

namespace SexyHttp.ArgumentHandlers
{
    public class StreamResponseArgumentHandler : HttpArgumentHandler
    {
        public StreamResponseArgumentHandler(ITypeConverter typeConverter) : base(typeConverter)
        {
        }

        public override Task ApplyArgument(HttpApiRequest request, string name, object argument)
        {
            if (request.ResponseContentTypeOverride != null)
                throw new Exception("The response content type has already been overridden.");

            request.ResponseContentTypeOverride = "application/octet-stream";

            return base.ApplyArgument(request, name, argument);
        }

        public async override Task ApplyArgument(HttpApiResponse response, string name, object argument)
        {
            var handler = (Func<Stream, Task>)argument;
            var stream = ((StreamHttpBody)response.Body).Stream;
            await handler(stream);

            await base.ApplyArgument(response, name, argument);
        }
    }
}
