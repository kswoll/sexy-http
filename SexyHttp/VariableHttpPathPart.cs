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
    }
}