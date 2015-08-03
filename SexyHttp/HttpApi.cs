using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SexyHttp.Urls;

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
            var path = HttpUrlParser.Parse(httpMethod.Path);
            var pathParameters = new HashSet<string>(path.PathParts.OfType<VariableHttpPathPart>().Select(x => x.Key));
            foreach (var parameter in method.GetParameters())
            {
                if (pathParameters.Contains(parameter.Name))
                {
                    
                }
            }

            var endpoint = new HttpApiEndpoint(path, httpMethod.Method, argumentHandlers, null);
            return endpoint;
        }

    }
}