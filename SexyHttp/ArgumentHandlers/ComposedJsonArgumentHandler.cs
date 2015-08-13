using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SexyHttp.TypeConverters;
using SexyHttp.HttpBodies;

namespace SexyHttp.ArgumentHandlers
{
    public class ComposedJsonArgumentHandler : HttpArgumentHandler
    {
        public string Name { get; }

        public ComposedJsonArgumentHandler(ITypeConverter typeConverter, string name = null) : base(typeConverter)
        {
            Name = name;
        }

        public override Task ApplyArgument(HttpApiRequest request, string name, object argument)
        {
            if (request.Body == null)
            {
                request.Body = new JsonHttpBody(new JObject());
            }
            else
            {
                if (!(request.Body is JsonHttpBody))
                    throw new Exception("ComposedJsonArgumentHandler expects either a null body or a JsonHttpBody");
                else if (!(((JsonHttpBody)request.Body).Json is JObject))
                    throw new Exception("ComposedJsonArgumentHandler expects a JsonHttpBody with a JObject value");
            }

            var jsonName = Name ?? name;
            var jsonObject = (JObject)((JsonHttpBody)request.Body).Json;
            var value = TypeConverter.ConvertTo<JToken>(TypeConversionContext.Body, argument);
            jsonObject[jsonName] = value;

            return base.ApplyArgument(request, name, argument);
        }
    }
}
