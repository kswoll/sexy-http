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

            var provider = (Func<Stream>)argument;
            var stream = provider();
            var streamBody = new StreamHttpBody(stream);
            request.Body = streamBody;

            return TaskConstants.Completed;
        }
    }
}
