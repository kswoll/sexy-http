using System.Collections.Generic;
using System.Reflection;

namespace SexyHttp
{
    public class HttpApi<T>
    {
        public IReadOnlyCollection<HttpApiEndpoint> Endpoints { get; }

        public HttpApi()
        {
            // Create endpoints
            var endpoints = new List<HttpApiEndpoint>();

            foreach (var method in typeof(T).GetMethods())
            {
                var httpMethod = method.GetHttpMethodAttribute();
                if (httpMethod != null)
                {
                    var endpoint = CreateEndpoint(method, httpMethod);
                    endpoints.Add(endpoint);                    
                }
            }

            Endpoints = endpoints;
        }

        protected HttpApiEndpoint CreateEndpoint(MethodInfo method, IHttpMethodAttribute httpMethod)
        {
            var argumentHandlers = new Dictionary<string, IHttpArgumentHandler>();
            var path = HttpPathParser.Parse(httpMethod.Path);
            foreach (var parameter in method.GetParameters())
            {
                
            }

            var endpoint = new HttpApiEndpoint(path, httpMethod.Method, argumentHandlers, null);
            return endpoint;
        }

    }
}