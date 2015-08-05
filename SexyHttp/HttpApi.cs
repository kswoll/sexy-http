using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SexyHttp.ArgumentHandlers;
using SexyHttp.TypeConverters;
using SexyHttp.Urls;

namespace SexyHttp
{
    public class HttpApi<T>
    {
        public IReadOnlyCollection<HttpApiEndpoint> Endpoints { get; }
        public ITypeConverter TypeConverter { get; }

        public HttpApi()
        {
            var typeConverterAttribute = typeof(T).GetCustomAttribute<TypeConverterAttribute>();
            TypeConverter = typeConverterAttribute != null ? (ITypeConverter)Activator.CreateInstance(typeConverterAttribute.ConverterType) : new DefaultTypeConverter();

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
            var url = HttpUrlParser.Parse(httpMethod.Path);
            var pathParameters = new HashSet<string>(url.PathParts.OfType<VariableHttpPathPart>().Select(x => x.Key));
            var queryParameters = new HashSet<string>(url.QueryParts.Select(x => x.Value).OfType<VariableHttpPathPart>().Select(x => x.Key));
            foreach (var parameter in method.GetParameters())
            {
                if (pathParameters.Contains(parameter.Name))
                {
                    argumentHandlers[parameter.Name] = new PathArgumentHandler(TypeConverter);
                }
                else if (queryParameters.Contains(parameter.Name))
                {
                    argumentHandlers[parameter.Name] = new QueryArgumentHandler(TypeConverter);
                }
            }

            var endpoint = new HttpApiEndpoint(url, httpMethod.Method, argumentHandlers, new NullResponseHandler());
            return endpoint;
        }
    }
}