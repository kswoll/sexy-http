using System.Collections.Generic;
using System.Linq;

namespace SexyHttp.ArgumentHandlers
{
    public class HttpHeaderArgumentHandler : IHttpArgumentHandler
    {
        private readonly string[] values;

        public HttpHeaderArgumentHandler(params string[] values)
        {
            this.values = values;
        }

        public HttpHeaderArgumentHandler(IEnumerable<string> values) : this(values.ToArray())
        {
        }

        public void ApplyArgument(HttpApiRequest request, string name, object argument)
        {
            request.Headers.Add(new HttpHeader(name, values));
        }
    }
}