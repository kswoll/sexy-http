using System.Collections.Generic;

namespace SexyHttp
{
    public class LiteralHttpPathPart : HttpPathPart
    {
        public string Value { get; }

        public LiteralHttpPathPart(string value)
        {
            Value = value;
        }
    }
}