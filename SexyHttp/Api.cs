using System.Collections.Generic;
using System.Threading.Tasks;
using SexyProxy;

namespace SexyHttp
{
    public class Api : IReverseProxy, ISetInvocationHandler
    {
        private InvocationHandler invocationHandler;

        InvocationHandler IReverseProxy.InvocationHandler => invocationHandler;

        InvocationHandler ISetInvocationHandler.InvocationHandler
        {
            set => invocationHandler = value;
        }

        public virtual Task<object> Call(HttpApiEndpoint endpoint, IHttpHandler httpHandler, string baseUrl, Dictionary<string, object> arguments, HttpApiInstrumenter apiInstrumenter)
        {
            return endpoint.Call(httpHandler, baseUrl, arguments, apiInstrumenter);
        }
    }
}
