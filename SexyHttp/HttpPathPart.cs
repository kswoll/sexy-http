using System.Collections.Generic;

namespace SexyHttp
{
    public abstract class HttpPathPart
    {
        public abstract string ToString(Dictionary<string, object> arguments);

        public static implicit operator HttpPathPart(string literal)
        {
            return new LiteralHttpPathPart(literal);
        }
    }
}