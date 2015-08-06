using System.Threading.Tasks;
using SexyHttp.TypeConverters;

namespace SexyHttp
{
    public interface IHttpResponseHandler
    {
        ITypeConverter TypeConverter { set; }
        Task<object> HandleResponse(HttpApiResponse response);
    }
}