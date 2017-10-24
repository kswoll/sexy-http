using System.Collections.Generic;
using System.Linq;

namespace SexyHttp
{
    public class HttpHeader
    {
        public string Name { get; }
        public IReadOnlyList<string> Values { get; }

        public HttpHeader(string name, params string[] values)
        {
            Name = name;
            Values = values.ToList();
        }
    }
}