using System.Collections.Generic;

namespace SexyHttp
{
    public class VariableHttpPathPart : IHttpPathPart
    {
        public string Key { get; }

        public VariableHttpPathPart(string key)
        {
            Key = key;
        }

        public string ToString(Dictionary<string, object> arguments)
        {
            return (string)arguments[Key];
        }
    }
}