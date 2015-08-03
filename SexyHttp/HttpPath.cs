using System.Collections.Generic;
using System.Text;

namespace SexyHttp
{
    public class HttpPath
    {
        private readonly HttpPathDescriptor descriptor;
        private readonly Dictionary<HttpPathPart, string> variables = new Dictionary<HttpPathPart, string>();

        public HttpPath(HttpPathDescriptor descriptor)
        {
            this.descriptor = descriptor;
        }

        public string this[HttpPathPart part]
        {
            get
            {
                string result;
                variables.TryGetValue(part, out result);
                return result;
            }
            set { variables[part] = value; }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var part in descriptor.Parts)
            {
                var literal = part as LiteralHttpPathPart;
                if (literal != null)
                    builder.Append(literal.Value);
                else
                    builder.Append(variables[part]);
            }
            return builder.ToString();
        }
    }
}