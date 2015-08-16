using System;
using System.Linq;
using System.Threading.Tasks;
using SexyHttp.HttpHandlers;
using SexyHttp.RequestInstrumenters;
using SexyProxy;

namespace SexyHttp
{
    public static class HttpApiClient<T>
    {
        public static T Create(string baseUrl, IHttpHandler httpHandler = null, IHttpApiRequestInstrumenter apiRequestInstrumenter = null)
        {
            return Create(new HttpApi<T>(), baseUrl, httpHandler, apiRequestInstrumenter);
        }

        public static T Create(HttpApi<T> api, string baseUrl, IHttpHandler httpHandler = null, IHttpApiRequestInstrumenter apiRequestInstrumenter = null)
        {
            httpHandler = httpHandler ?? new HttpClientHttpHandler();
            var clientHandler = new ClientHandler(api, baseUrl, httpHandler, apiRequestInstrumenter);
            T proxy = Proxy.CreateProxy<T>(clientHandler.Call);
            clientHandler.SetProxy(proxy);
            return proxy;
        }

        private class ClientHandler
        {
            private readonly HttpApi<T> api;
            private readonly string baseUrl;
            private readonly IHttpHandler httpHandler;
            private IHttpApiRequestInstrumenter apiRequestInstrumenter;
            private T proxy;

            public ClientHandler(HttpApi<T> api, string baseUrl, IHttpHandler httpHandler, IHttpApiRequestInstrumenter apiRequestInstrumenter)
            {
                this.api = api;
                this.baseUrl = baseUrl;
                this.httpHandler = httpHandler;
                this.apiRequestInstrumenter = apiRequestInstrumenter;
            }

            internal void SetProxy(T proxy)
            {
                this.proxy = proxy;

                if (typeof(IHttpApiRequestInstrumenter).IsAssignableFrom(typeof(T)))
                {
                    if (apiRequestInstrumenter == null)
                        apiRequestInstrumenter = (IHttpApiRequestInstrumenter)proxy;
                    else
                        apiRequestInstrumenter = new CombinedRequestInstrumenter(apiRequestInstrumenter, (IHttpApiRequestInstrumenter)proxy);
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
                    return ((Api)(object)proxy).Call(endpoint, httpHandler, baseUrl, arguments, apiRequestInstrumenter);
                }
                else
                {
                    var call = endpoint.Call(httpHandler, baseUrl, arguments, apiRequestInstrumenter);
                    return call;
                }
            }
        }
    }
}