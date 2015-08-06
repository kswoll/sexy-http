using System.Threading.Tasks;

namespace SexyHttp.HttpBodies 
{
    public class ByteArrayHttpBody : HttpBody
    {
        public byte[] Data { get; set; }

        public ByteArrayHttpBody(byte[] data = null)
        {
            Data = data;
        }

        public override T Accept<T>(IHttpBodyVisitor<T> visitor)
        {
            return visitor.VisitByteArrayBody(this);
        }

        public override Task<T> AcceptAsync<T>(IAsyncHttpBodyVisitor<T> visitor)
        {
            return visitor.VisitByteArrayBodyAsync(this);
        }
    }
}
