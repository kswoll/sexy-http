using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SexyHttp.HttpBodies
{
    public class MultipartHttpBody : HttpBody
    {
        public Dictionary<string, MultipartData> Data { get; set; }

        public MultipartHttpBody()
        {
            Data = new Dictionary<string, MultipartData>();
        }

        public override T Accept<T>(IHttpBodyVisitor<T> visitor)
        {
            return visitor.VisitMultipartBody(this);
        }

        public override Task<T> AcceptAsync<T>(IAsyncHttpBodyVisitor<T> visitor)
        {
            return visitor.VisitMultipartBodyAsync(this);
        }

        public override string ToString()
        {
            return string.Join("\r\n", Data.Select(x => $"{x.Key}={x.Value}"));
        }
    }
}
