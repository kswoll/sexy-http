using System;
using Newtonsoft.Json.Linq;
using SexyHttp.ArgumentHandlers;
using SexyHttp.HttpBodies;

namespace SexyHttp.TypeConverters
{
    public class DefaultTypeConverter : CombinedTypeConverter
    {
        public DefaultTypeConverter() : base(CreateRegistryTypeConverter(), new IdentityTypeConverter(), new ArrayTypeConverter())
        {
        }

        private static RegistryTypeConverter CreateRegistryTypeConverter()
        {
            var result = new RegistryTypeConverter();

            result.Register<object, JToken>(LambdaTypeConverter.Create(x => JToken.FromObject(x)));
            result.Register<string, HttpBody>(LambdaTypeConverter.Create(x => new StringHttpBody((string)x)));
            result.Register<byte[], HttpBody>(LambdaTypeConverter.Create(x => new ByteArrayHttpBody((byte[])x)));
            result.Register<Type, IHttpResponseHandler>(new HttpResponseTypeConverter());

            return result;
        }
    }
}
