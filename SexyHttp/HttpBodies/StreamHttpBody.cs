using System.IO;
using System.Threading.Tasks;

namespace SexyHttp.HttpBodies
{
    public class StreamHttpBody : HttpBody
    {
        public Stream Stream { get; set; }

        public StreamHttpBody(Stream stream = null)
        {
            Stream = stream;
        }

        public override T Accept<T>(IHttpBodyVisitor<T> visitor)
        {
            return visitor.VisitStreamBody(this);
        }

        public override Task<T> AcceptAsync<T>(IAsyncHttpBodyVisitor<T> visitor)
        {
            return visitor.VisitStreamBodyAsync(this);
        }

        public override string ToString()
        {
            return "(Stream)";
        }
    }
}
