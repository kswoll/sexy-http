using System;
using System.Threading.Tasks;
using SexyHttp.TypeConverters;

namespace SexyHttp
{
    public abstract class HttpResponseHandler : IHttpResponseHandler
    {
        public abstract Task<object> HandleResponse(HttpApiResponse response);

        public virtual ITypeConverter TypeConverter { get; set; }
        public virtual Type ResponseType { get; set;  }
    }
}
