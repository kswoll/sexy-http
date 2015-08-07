using System.Threading.Tasks;
using SexyHttp.TypeConverters;

namespace SexyHttp
{
    public interface IHttpArgumentHandler
    {
        Task ApplyArgument(HttpApiRequest request, string name, object argument);
        Task ApplyArgument(HttpApiResponse response, string name, object argument);
    }
}