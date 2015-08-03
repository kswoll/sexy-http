using System.Collections.Generic;

namespace SexyHttp
{
    public class LiteralHttpPathPart : IHttpPathPart
    {
        public string Value { get; }

        public LiteralHttpPathPart(string value)
        {
            Value = value;
        }

        public string ToString(Dictionary<string, object> arguments)
        {
            return Value;
        }
    }
}