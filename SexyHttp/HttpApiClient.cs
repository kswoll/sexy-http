using System;
using System.Linq;
using System.Threading.Tasks;
using SexyHttp.HttpHandlers;
using SexyHttp.Instrumenters;
using SexyHttp.TypeConverters;
using SexyProxy;

namespace SexyHttp
{
    /// <summary>
    /// Use this class to create an instance of your API client via the Create methods.
    /// </summary>
    /// <typeparam name="T">The type of your interface that defines the methods of your API</typeparam>
    public static class HttpApiClient<T>
    {
        /// <summary>
        /// Creates a new client for your API.  Usually you should use the overload that doesn't require you to pass
        /// in the instance of HttpApi.
        /// </summary>
        /// <param name="baseUrl">The base URL that will be prepended to the constructed API paths dictated
        /// by the attributes associated with your API methods.</param>
        /// <param name="typeConverter">An instance of ITypeConverter that allows you to control how
        /// parameters and returned values are converted from lower-level types such as JSON values to
        /// higher level types such as ordinary C# types, and vice versa.</param>
        /// <param name="httpHandler">The instance of IHttpHandler that is responsible for the actual network
        /// IO.  There are two primary implementations: HttpClientHttpHandler, useful when you're using async
        /// code and WebRequestHttpHandler, useful when you'd prefer not to use async patterns.</param>
        /// <param name="apiInstrumenter">An optional callback that will be invoked when about to make the
        /// call to the server.  You can use this to both modify the incoming HTTP request, and also to
        /// modify the HTTP response that is returned.  You must call the "inner" callback provided to
        /// your callback to actually make the call.</param>
        /// <param name="interfaceHandler">Similar to the instrumenter above, but more high-level in that you
        /// have access to the method arguments passed to the API method, and access to the object returned
        /// from the API after undergoing deserialization.</param>
        /// <returns>An instance of your API interface that you can use to make calls to your backend by
        /// invoking its methods.</returns>
        public static T Create(string baseUrl, ITypeConverter typeConverter = null, IHttpHandler httpHandler = null, HttpApiInstrumenter apiInstrumenter = null, Func<Invocation, Task<object>> interfaceHandler = null)
        {
            return Create(new HttpApi<T>(typeConverter), baseUrl, httpHandler, apiInstrumenter, interfaceHandler);
        }

        /// <summary>
        /// Creates a new client for your API.  Usually you should use the overload that doesn't require you to pass
        /// in the instance of HttpApi.
        /// </summary>
        /// <param name="api">The instance of HttpApi that is responsible for deconstructing your API interface
        /// into a series of HttpApiEndpoint instances -- one per method defined in your API.</param>
        /// <param name="baseUrl">The base URL that will be prepended to the constructed API paths dictated
        /// by the attributes associated with your API methods.</param>
        /// <param name="httpHandler">The instance of IHttpHandler that is responsible for the actual network
        /// IO.  There are two primary implementations: HttpClientHttpHandler, useful when you're using async
        /// code and WebRequestHttpHandler, useful when you'd prefer not to use async patterns.</param>
        /// <param name="apiInstrumenter">An optional callback that will be invoked when about to make the
        /// call to the server.  You can use this to both modify the incoming HTTP request, and also to
        /// modify the HTTP response that is returned.  You must call the "inner" callback provided to
        /// your callback to actually make the call.</param>
        /// <param name="interfaceHandler">Similar to the instrumenter above, but more high-level in that you
        /// have access to the method arguments passed to the API method, and access to the object returned
        /// from the API after undergoing deserialization.</param>
        /// <returns>An instance of your API interface that you can use to make calls to your backend by
        /// invoking its methods.</returns>
        public static T Create(HttpApi<T> api, string baseUrl, IHttpHandler httpHandler = null, HttpApiInstrumenter apiInstrumenter = null, Func<Invocation, Task<object>> interfaceHandler = null)
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
            private HttpApiInstrumenter apiInstrumenter;
            private T proxy;

            public ClientHandler(HttpApi<T> api, string baseUrl, IHttpHandler httpHandler, HttpApiInstrumenter apiInstrumenter)
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
                    Task<HttpApiResponse> ClassInstrumenter(HttpApiRequest request, Func<HttpApiRequest, Task<HttpApiResponse>> inner)
                    {
                        return ((IHttpApiInstrumenter)proxy).InstrumentCall(request, inner);
                    }

                    if (apiInstrumenter == null)
                        apiInstrumenter = ClassInstrumenter;
                    else
                        apiInstrumenter = new CombinedInstrumenter(apiInstrumenter, ClassInstrumenter).InstrumentCall;
                }
            }

            public Task<object> Call(Invocation invocation)
            {
                if (!api.Endpoints.TryGetValue(invocation.Method, out var endpoint))
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