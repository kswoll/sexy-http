using System;
using System.IO;
using System.Threading.Tasks;
using SexyHttp.HttpBodies;
using SexyHttp.TypeConverters;
using SexyHttp.Utils;

namespace SexyHttp.ArgumentHandlers
{
    public class StreamArgumentHandler : HttpArgumentHandler
    {
        public StreamArgumentHandler(ITypeConverter typeConverter) : base(typeConverter)
        {
        }

        public override Task ApplyArgument(HttpApiRequest request, string name, object argument)
        {
            if (request.Body != null)
                throw new Exception("Can only use StreamArgumentHandler for one argument.  If you need multiple arguments, you " +
                                    "should be using MultipartStreamArgumentHandler, which ought to have been selected " +
                                    "automatically under default conditions.");

            var streamBody = new StreamHttpBody((Stream)argument);
            request.Body = streamBody;

            return base.ApplyArgument(request, name, argument);
        }
    }
}
