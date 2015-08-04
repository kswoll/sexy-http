using SexyHttp.TypeConverters;

namespace SexyHttp.ArgumentHandlers
{
    public class QueryArgumentHandler : HttpArgumentHandler
    {
        public QueryArgumentHandler(ITypeConverter typeConverter) : base(typeConverter)
        {
        }

        public override void ApplyArgument(HttpApiRequest request, string name, object argument)
        {
            var value = TypeConverter.ConvertTo<string[]>(argument);
            request.Url.Query[name] = value;
        }
    }
}
