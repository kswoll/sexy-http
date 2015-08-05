using System.Threading.Tasks;

namespace SexyHttp.HttpBodies
{
    public class StringHttpBody : HttpBody
    {
        public string Text { get; set; }

        public StringHttpBody(string text = null)
        {
            Text = text;
        }

        public override T Accept<T>(IHttpBodyVisitor<T> visitor)
        {
            return visitor.VisitStringBody(this);
        }

        public override Task<T> AcceptAsync<T>(IAsyncHttpBodyVisitor<T> visitor)
        {
            return visitor.VisitStringBodyAsync(this);
        }
    }
}
