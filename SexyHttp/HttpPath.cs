using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SexyHttp
{
    public sealed class HttpPath
    {
        private readonly List<IHttpPathPart> parts;

        public HttpPath(IEnumerable<IHttpPathPart> parts)
        {
            this.parts = parts.ToList();
        }

        public HttpPath(params IHttpPathPart[] parts)
        {
            this.parts = parts.ToList();
        }

        public string ToString(Dictionary<string, object> arguments)
        {
            var builder = new StringBuilder();
            foreach (var part in parts)
            {
                builder.Append(part.ToString(arguments));
            }
            return builder.ToString();
        }

        public static implicit operator HttpPath(string path)
        {
            return new HttpPath(new LiteralHttpPathPart(path));
        }
    }
}