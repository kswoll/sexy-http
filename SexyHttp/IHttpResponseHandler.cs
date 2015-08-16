using System;
using System.Threading.Tasks;
using SexyHttp.TypeConverters;

namespace SexyHttp
{
    public interface IHttpResponseHandler
    {
        Type ResponseType { set; }
        ITypeConverter TypeConverter { set; }
        Task<object> HandleResponse(HttpApiRequest request, HttpApiResponse response);
    }
}