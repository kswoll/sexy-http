using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SexyHttp
{
    public sealed class HttpPath
    {
        public IReadOnlyList<HttpPathPart> Parts { get; }

        public HttpPath(IEnumerable<HttpPathPart> parts)
        {
            Parts = parts.ToList();
        }

        public HttpPath(params HttpPathPart[] parts)
        {
            Parts = parts.ToList();
        }

        public string ToString(Dictionary<string, object> arguments)
        {
            var builder = new StringBuilder();
            foreach (var part in Parts)
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