using System.Threading.Tasks;
using SexyProxy;

namespace SexyHttp
{
    /// <summary>
    /// There are two primary ways to create an API interface.  One is to just use an interface; this
    /// is the usual technique.  However, if you instead subclass Api, you have a lot of power to
    /// affect the implementation of individual API methods by instead surfacing it as a virtual method
    /// and able to deal with the arguments and return value in a far more powerful way.
    /// </summary>
    public class Api : IReverseProxy, ISetInvocationHandler
    {
        private InvocationHandler invocationHandler;

        InvocationHandler IReverseProxy.InvocationHandler => invocationHandler;

        InvocationHandler ISetInvocationHandler.InvocationHandler
        {
            set => invocationHandler = value;
        }

        public virtual Task<object> Call(HttpApiEndpoint endpoint, IHttpHandler httpHandler, string baseUrl, HttpApiArguments arguments, HttpApiInstrumenter apiInstrumenter)
        {
            return endpoint.Call(httpHandler, baseUrl, arguments, apiInstrumenter);
        }
    }
}
