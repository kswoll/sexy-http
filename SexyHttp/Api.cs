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
    }
}
