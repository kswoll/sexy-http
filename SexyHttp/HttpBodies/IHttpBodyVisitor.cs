using System.Threading.Tasks;

namespace SexyHttp.HttpBodies
{
    public interface IHttpBodyVisitor<out T>
    {
        T VisitJsonBody(JsonHttpBody body);
        T VisitStringBody(StringHttpBody body);
    }
}
