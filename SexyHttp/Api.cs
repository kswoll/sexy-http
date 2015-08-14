using System.Collections.Generic;
using System.Threading.Tasks;
using SexyProxy;

namespace SexyHttp
{
    public class Api : IProxy, ISetInvocationHandler
    {
        private InvocationHandler invocationHandler;

        InvocationHandler IProxy.InvocationHandler => invocationHandler;

        InvocationHandler ISetInvocationHandler.InvocationHandler
        {
            set { invocationHandler = value; }
        }

        public virtual Task<object> Call(HttpApiEndpoint endpoint, IHttpHandler httpHandler, string baseUrl, Dictionary<string, object> arguments, IHttpApiRequestInstrumenter apiRequestInstrumenter)
        {
            return endpoint.Call(httpHandler, baseUrl, arguments, apiRequestInstrumenter);
        }
    }
}
