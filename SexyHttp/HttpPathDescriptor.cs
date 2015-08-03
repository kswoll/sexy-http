using System.Collections.Generic;
using System.Linq;

namespace SexyHttp
{
    public sealed class HttpPathDescriptor
    {
        public IReadOnlyList<HttpPathPart> Parts { get; }

        public HttpPathDescriptor(IEnumerable<HttpPathPart> parts)
        {
            Parts = parts.ToList();
        }

        public HttpPathDescriptor(params HttpPathPart[] parts)
        {
            Parts = parts.ToList();
        }

        public static implicit operator HttpPathDescriptor(string path)
        {
            return new HttpPathDescriptor(new LiteralHttpPathPart(path));
        }

        public HttpPath CreatePath()
        {
            return new HttpPath(this);
        }
    }
}