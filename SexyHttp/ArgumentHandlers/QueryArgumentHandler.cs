using System.Threading.Tasks;
using SexyHttp.TypeConverters;
using SexyHttp.Utils;

namespace SexyHttp.ArgumentHandlers
{
    public class QueryArgumentHandler : HttpArgumentHandler
    {
        public QueryArgumentHandler(ITypeConverter typeConverter) : base(typeConverter)
        {
        }

        public override Task ApplyArgument(HttpApiRequest request, string name, object argument)
        {
            var value = TypeConverter.ConvertTo<string[]>(argument);
            request.Url.Query[name] = value;

            return base.ApplyArgument(request, name, argument);
        }
    }
}
