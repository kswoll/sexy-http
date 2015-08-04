using SexyHttp.TypeConverters;

namespace SexyHttp.ArgumentHandlers
{
    public class PathArgumentResolver : HttpArgumentHandler
    {
        public PathArgumentResolver(ITypeConverter typeConverter) : base(typeConverter)
        {
        }

        public override void ApplyArgument(HttpApiRequest request, string name, object argument)
        {
            var value = TypeConverter.ConvertTo<string>(argument);
            request.Url.Path[name] = value;
        }
    }
}
