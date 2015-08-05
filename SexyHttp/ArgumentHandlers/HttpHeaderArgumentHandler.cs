using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SexyHttp.Utils;

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

        public Task ApplyArgument(HttpApiRequest request, string name, object argument)
        {
            request.Headers.Add(new HttpHeader(name, values));
            return TaskConstants.Completed;
        }
    }
}