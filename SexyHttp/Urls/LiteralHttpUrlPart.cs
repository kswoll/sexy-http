using System.Collections.Generic;

namespace SexyHttp.Urls
{
    public class LiteralHttpUrlPart : HttpUrlPart
    {
        public string Value { get; }

        public LiteralHttpUrlPart(string value)
        {
            Value = value;
        }
    }
}