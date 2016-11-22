using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SexyHttp.HttpBodies;

namespace SexyHttp.TypeConverters
{
    public class DefaultTypeConverter : CombinedTypeConverter
    {
        private DefaultTypeConverter(params ITypeConverter[] typeConverters) : base(typeConverters)
        {
        }

        public static DefaultTypeConverter Create()
        {
            var registry = new RegistryTypeConverter();
            var result = new DefaultTypeConverter(registry, new IdentityTypeConverter(), new ElementToArrayTypeConverter(), new SystemConvertTypeConverter());

            registry.Register<object, JToken>(LambdaTypeConverter.Create(x => x == null ? null : JToken.FromObject(x)));
            registry.Register<JToken, object>(LambdaTypeConverter.Create((x, type) => ((JToken)x).ToObject(type)));
            registry.Register<string, HttpBody>(LambdaTypeConverter.Create(x => new StringHttpBody((string)x)));
            registry.Register<byte[], HttpBody>(LambdaTypeConverter.Create(x => new ByteArrayHttpBody((byte[])x)));
            registry.Register<Stream, HttpBody>(LambdaTypeConverter.Create(x => new StreamHttpBody((Stream)x)));
            registry.Register<Array, Array>(new ArrayTypeConverter());

            return result;
        }
    }
}
