using System.Threading.Tasks;
using SexyHttp.TypeConverters;
using SexyHttp.Utils;

namespace SexyHttp.ArgumentHandlers
{
    public class HttpHeaderArgumentHandler : HttpArgumentHandler
    {
        public string Name { get; }
        public string[] Values { get; }

        public HttpHeaderArgumentHandler(ITypeConverter typeConverter, string name = null) : base(typeConverter)
        {
            Name = name;
        }

        public HttpHeaderArgumentHandler(ITypeConverter typeConverter, string name, params string[] values) : base(typeConverter)
        {
            Name = name;
            Values = values;
        }

        public override Task ApplyArgument(HttpApiRequest request, string name, object argument)
        {
            name = Name ?? name;

            var values = Values ?? TypeConverter.ConvertTo<string[]>(TypeConversionContext.Header, argument);

            request.Headers.Add(new HttpHeader(name, values));
            return base.ApplyArgument(request, name, argument);
        }
    }
}