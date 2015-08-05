using System.Linq;
using System.Threading.Tasks;
using SexyProxy;

namespace SexyHttp
{
    public static class HttpApiClient<T>
    {
        public static T Create(HttpApi<T> api, string baseUrl, IHttpHandler httpHandler, IHttpHeadersProvider headersProvider)
        {
            return Proxy.CreateProxy<T>(new ClientHandler(api, baseUrl, httpHandler, headersProvider).Call);
        }

        private class ClientHandler
        {
            private readonly HttpApi<T> api;
            private readonly string baseUrl;
            private readonly IHttpHandler httpHandler;
            private readonly IHttpHeadersProvider headersProvider;

            public ClientHandler(HttpApi<T> api, string baseUrl, IHttpHandler httpHandler, IHttpHeadersProvider headersProvider)
            {
                this.api = api;
                this.baseUrl = baseUrl;
                this.httpHandler = httpHandler;
                this.headersProvider = headersProvider;
            }

            public Task<object> Call(Invocation invocation)
            {
                var endpoint = api.Endpoints[invocation.Method];
                return endpoint.Call(httpHandler, headersProvider, baseUrl, invocation.Method
                    .GetParameters()
                    .Select((x, i) => new { x.Name, Value = invocation.Arguments[i] })
                    .ToDictionary(x => x.Name, x => x.Value));
            }
        }
    }
}