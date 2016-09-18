using System;
using System.Linq;
using System.Threading.Tasks;
using SexyHttp.HttpHandlers;
using SexyHttp.Instrumenters;
using SexyProxy;

namespace SexyHttp
{
    public static class HttpApiClient<T>
    {
        public static T Create(string baseUrl, IHttpHandler httpHandler = null, IHttpApiInstrumenter apiInstrumenter = null, Func<Invocation, Task<object>> interfaceHandler = null)
        {
            return Create(new HttpApi<T>(), baseUrl, httpHandler, apiInstrumenter, interfaceHandler);
        }

        public static T Create(HttpApi<T> api, string baseUrl, IHttpHandler httpHandler = null, IHttpApiInstrumenter apiInstrumenter = null, Func<Invocation, Task<object>> interfaceHandler = null)
        {
            httpHandler = httpHandler ?? new HttpClientHttpHandler();
            var clientHandler = new ClientHandler(api, baseUrl, httpHandler, apiInstrumenter);
            T proxy = Proxy.CreateProxy<T>(clientHandler.Call);

            if (interfaceHandler != null)
            {
                // If the interfaceHandler is specified, we allow it to intercept each call. It can invoke Proceed itself if it
                // wants the default implementation.
                proxy = Proxy.CreateProxy(proxy, invocation => interfaceHandler(invocation));
            }
            clientHandler.SetProxy(proxy);

            return proxy;
        }

        private class ClientHandler
        {
            private readonly HttpApi<T> api;
            private readonly string baseUrl;
            private readonly IHttpHandler httpHandler;
            private IHttpApiInstrumenter apiInstrumenter;
            private T proxy;

            public ClientHandler(HttpApi<T> api, string baseUrl, IHttpHandler httpHandler, IHttpApiInstrumenter apiInstrumenter)
            {
                this.api = api;
                this.baseUrl = baseUrl;
                this.httpHandler = httpHandler;
                this.apiInstrumenter = apiInstrumenter;
            }

            internal void SetProxy(T proxy)
            {
                this.proxy = proxy;

                if (typeof(IHttpApiInstrumenter).IsAssignableFrom(typeof(T)))
                {
                    if (apiInstrumenter == null)
                        apiInstrumenter = (IHttpApiInstrumenter)proxy;
                    else
                        apiInstrumenter = new CombinedInstrumenter(apiInstrumenter, (IHttpApiInstrumenter)proxy);
                }
            }

            public Task<object> Call(Invocation invocation)
            {
                HttpApiEndpoint endpoint;
                if (!api.Endpoints.TryGetValue(invocation.Method, out endpoint))
                    throw new Exception($"Endpoint not found for: \"{invocation.Method.DeclaringType.FullName}.{invocation.Method.Name}\".  Perhaps you forgot to decorate your method with [Get], [Post], etc.");

                var arguments = invocation.Method
                    .GetParameters()
                    .Select((x, i) => new { x.Name, Value = invocation.Arguments[i] })
                    .ToDictionary(x => x.Name, x => x.Value);
                if (proxy is Api)
                {
                    return ((Api)(object)proxy).Call(endpoint, httpHandler, baseUrl, arguments, apiInstrumenter);
                }
                else
                {
                    var call = endpoint.Call(httpHandler, baseUrl, arguments, apiInstrumenter);
                    return call;
                }
            }
        }
    }
}