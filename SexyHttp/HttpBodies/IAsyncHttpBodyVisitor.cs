using System.Threading.Tasks;

namespace SexyHttp.HttpBodies
{
    public interface IAsyncHttpBodyVisitor<T>
    {
        Task<T> VisitJsonBodyAsync(JsonHttpBody body);
        Task<T> VisitStringBodyAsync(StringHttpBody body);
        Task<T> VisitMultipartBodyAsync(MultipartHttpBody body);
        Task<T> VisitByteArrayBodyAsync(ByteArrayHttpBody body);
        Task<T> VisitStreamBodyAsync(StreamHttpBody body);
    }
}
