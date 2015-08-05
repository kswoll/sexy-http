using Newtonsoft.Json.Linq;

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

            return result;
        }
    }
}
