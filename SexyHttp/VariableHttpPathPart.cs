using System.Collections.Generic;

namespace SexyHttp
{
    public class VariableHttpPathPart : HttpPathPart
    {
        public string Key { get; }

        public VariableHttpPathPart(string key)
        {
            Key = key;
        }

        public override string ToString(Dictionary<string, object> arguments)
        {
            return (string)arguments[Key];
        }
    }
}