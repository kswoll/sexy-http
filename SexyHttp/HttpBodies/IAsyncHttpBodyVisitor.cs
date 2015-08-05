using System.Threading.Tasks;

namespace SexyHttp.HttpBodies
{
    public interface IAsyncHttpBodyVisitor<T>
    {
        Task<T> VisitJsonBodyAsync(JsonHttpBody body);
        Task<T> VisitStringBodyAsync(StringHttpBody body);
    }
}
