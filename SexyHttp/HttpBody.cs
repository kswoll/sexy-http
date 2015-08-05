using System.Threading.Tasks;
using SexyHttp.HttpBodies;

namespace SexyHttp
{
    public abstract class HttpBody
    {
        public abstract T Accept<T>(IHttpBodyVisitor<T> visitor);
        public abstract Task<T> AcceptAsync<T>(IAsyncHttpBodyVisitor<T> visitor);
    }
}