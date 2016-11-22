using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SexyHttp.HttpBodies
{
    public class JsonHttpBody : HttpBody
    {
        public JToken Json { get; }

        public JsonHttpBody(JToken json)
        {
            Json = json;
        }

        public override T Accept<T>(IHttpBodyVisitor<T> visitor)
        {
            return visitor.VisitJsonBody(this);
        }

        public override Task<T> AcceptAsync<T>(IAsyncHttpBodyVisitor<T> visitor)
        {
            return visitor.VisitJsonBodyAsync(this);
        }

        public override string ToString()
        {
            return Json.ToString();
        }
    }
}
