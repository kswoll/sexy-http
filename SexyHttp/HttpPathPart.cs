using System.Collections.Generic;

namespace SexyHttp
{
    public abstract class HttpPathPart
    {
        public static implicit operator HttpPathPart(string literal)
        {
            return new LiteralHttpPathPart(literal);
        }
    }
}