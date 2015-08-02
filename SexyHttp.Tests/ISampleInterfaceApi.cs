using System.Threading.Tasks;
using SexyProxy;

namespace SexyHttp.Tests
{
    [Proxy]
    public interface ISampleInterfaceApi 
    {
        [Get("path/to/get")]
        Task<string> SimpleGetString();
    }
}