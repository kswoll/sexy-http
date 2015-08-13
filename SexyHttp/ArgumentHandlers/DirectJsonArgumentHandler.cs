using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SexyHttp.HttpBodies;
using SexyHttp.TypeConverters;
using SexyHttp.Utils;

namespace SexyHttp.ArgumentHandlers
{
    public class DirectJsonArgumentHandler : HttpArgumentHandler
    {
        public DirectJsonArgumentHandler(ITypeConverter typeConverter) : base(typeConverter)
        {
        }

        public override Task ApplyArgument(HttpApiRequest request, string name, object argument)
        {
            if (request.Body != null)
                throw new Exception("DirectJsonArgumentHandler expects a null request body.");

            var token = TypeConverter.ConvertTo<JToken>(TypeConversionContext.Body, argument);
            request.Body = new JsonHttpBody(token);

            return base.ApplyArgument(request, name, argument);
        }
    }
}
