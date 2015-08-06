using System;
using System.Linq;
using System.Threading.Tasks;
using SexyProxy;

namespace SexyHttp
{
    public static class HttpApiClient<T>
    {
        public static T Create(string baseUrl, IHttpHandler httpHandler, IHttpApiRequestInstrumenter apiRequestInstrumenter = null)
        {
            return Create(new HttpApi<T>(), baseUrl, httpHandler, apiRequestInstrumenter);
        }

        public static T Create(HttpApi<T> api, string baseUrl, IHttpHandler httpHandler, IHttpApiRequestInstrumenter apiRequestInstrumenter = null)
        {
            return Proxy.CreateProxy<T>(new ClientHandler(api, baseUrl, httpHandler, apiRequestInstrumenter).Call);
        }

        private class ClientHandler
        {
            private readonly HttpApi<T> api;
            private readonly string baseUrl;
            private readonly IHttpHandler httpHandler;
            private readonly IHttpApiRequestInstrumenter apiRequestInstrumenter;

            public ClientHandler(HttpApi<T> api, string baseUrl, IHttpHandler httpHandler, IHttpApiRequestInstrumenter apiRequestInstrumenter)
            {
                this.api = api;
                this.baseUrl = baseUrl;
                this.httpHandler = httpHandler;
                this.apiRequestInstrumenter = apiRequestInstrumenter;
            }

            public Task<object> Call(Invocation invocation)
            {
                HttpApiEndpoint endpoint;
                if (!api.Endpoints.TryGetValue(invocation.Method, out endpoint))
                    throw new Exception($"Endpoint not found for: {invocation.Method.DeclaringType.FullName}.{invocation.Method.Name}");
                return endpoint.Call(httpHandler, baseUrl, invocation.Method
                    .GetParameters()
                    .Select((x, i) => new { x.Name, Value = invocation.Arguments[i] })
                    .ToDictionary(x => x.Name, x => x.Value), apiRequestInstrumenter);
            }
        }
    }
}