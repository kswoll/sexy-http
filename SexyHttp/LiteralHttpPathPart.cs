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

        public override string ToString(Dictionary<string, object> arguments)
        {
            return Value;
        }
    }
}