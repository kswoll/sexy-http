using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SexyHttp.HttpBodies
{
    public class FormHttpBody : HttpBody
    {
        public Dictionary<string, string> Values { get; set; }

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
    }
}