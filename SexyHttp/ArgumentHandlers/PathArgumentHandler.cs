using SexyHttp.TypeConverters;

namespace SexyHttp.ArgumentHandlers
{
    public class PathArgumentHandler : HttpArgumentHandler
    {
        public PathArgumentHandler(ITypeConverter typeConverter) : base(typeConverter)
        {
        }

        public override void ApplyArgument(HttpApiRequest request, string name, object argument)
        {
            var value = TypeConverter.ConvertTo<string>(argument);
            request.Url.Path[name] = value;
        }
    }
}
