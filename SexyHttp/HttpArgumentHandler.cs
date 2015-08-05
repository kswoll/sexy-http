using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SexyHttp.TypeConverters;

namespace SexyHttp
{
    public abstract class HttpArgumentHandler : IHttpArgumentHandler
    {
        public abstract Task ApplyArgument(HttpApiRequest request, string name, object argument);

        public ITypeConverter TypeConverter { get; }

        protected HttpArgumentHandler(ITypeConverter typeConverter)
        {
            TypeConverter = typeConverter;
        }
    }
}
