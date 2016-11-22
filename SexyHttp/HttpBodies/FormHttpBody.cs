using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SexyHttp.Utils;

namespace SexyHttp.HttpBodies
{
    public class FormHttpBody : HttpBody
    {
        public Dictionary<string, string> Values { get; set; }

        public FormHttpBody(object values) : this(values.ObjectToDictionary(x => x.ToString()))
        {
        }

        public FormHttpBody(Dictionary<string, string> values = null)
        {
            Values = values ?? new Dictionary<string, string>();
        }

        public override T Accept<T>(IHttpBodyVisitor<T> visitor)
        {
            return visitor.VisitFormBody(this);
        }

        public override Task<T> AcceptAsync<T>(IAsyncHttpBodyVisitor<T> visitor)
        {
            return visitor.VisitFormBodyAsync(this);
        }

        public override string ToString()
        {
            return string.Join("\r\n", Values.Select(x => $"{x.Key}={x.Value}"));
        }
    }
}