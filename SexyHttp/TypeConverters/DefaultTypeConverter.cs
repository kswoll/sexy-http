using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SexyHttp.HttpBodies;
using SexyHttp.Urls;

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
            registry.Register<Enum, string>(LambdaTypeConverter.Create(x => EnumMemberCache.GetEnumMemberName((Enum)x)));
            registry.Register<string, Enum>(LambdaTypeConverter.Create((x, type) => EnumMemberCache.GetEnumMemberByName(type, (string)x)));
            registry.Register<bool, string>(LambdaTypeConverter.Create(x => (bool)x ? "true" : "false"));
            registry.Register<string, bool>(LambdaTypeConverter.Create(x => bool.Parse((string)x)));

            return result;
        }
    }
}
