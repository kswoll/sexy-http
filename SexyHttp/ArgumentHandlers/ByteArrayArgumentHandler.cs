using System;
using System.IO;
using System.Threading.Tasks;
using SexyHttp.HttpBodies;
using SexyHttp.TypeConverters;

namespace SexyHttp.ArgumentHandlers
{
    public class ByteArrayArgumentHandler : HttpArgumentHandler
    {
        public ByteArrayArgumentHandler(ITypeConverter typeConverter) : base(typeConverter)
        {
        }

        public override Task ApplyArgument(HttpApiRequest request, string name, object argument)
        {
            if (request.Body != null)
                throw new Exception("Can only use ByteArrayArgumentHandler for one argument.  If you need multiple arguments, you " +
                                    "should be using MultipartStreamArgumentHandler, which ought to have been selected " +
                                    "automatically under default conditions.");

            var streamBody = new ByteArrayHttpBody((byte[])argument);
            request.Body = streamBody;

            return base.ApplyArgument(request, name, argument);
        }
    }
}
