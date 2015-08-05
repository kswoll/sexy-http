using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
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
            TypeConverter = TypeConverterAttribute.GetTypeConverter(typeof(T)) ?? new DefaultTypeConverter();

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
            var typeConverter = TypeConverterAttribute.Combine(method, TypeConverter);
            var argumentHandlers = new Dictionary<string, IHttpArgumentHandler>();
            var url = HttpUrlParser.Parse(httpMethod.Path);
            var pathParameters = new HashSet<string>(url.PathParts.OfType<VariableHttpPathPart>().Select(x => x.Key));
            var queryParameters = new HashSet<string>(url.QueryParts.Select(x => x.Value).OfType<VariableHttpPathPart>().Select(x => x.Key));
            var bodyParameters = new List<ParameterInfo>();
            foreach (var parameter in method.GetParameters())
            {
                typeConverter = TypeConverterAttribute.Combine(parameter, typeConverter);
                if (pathParameters.Contains(parameter.Name))
                {
                    argumentHandlers[parameter.Name] = new PathArgumentHandler(typeConverter);
                }
                else if (queryParameters.Contains(parameter.Name))
                {
                    argumentHandlers[parameter.Name] = new QueryArgumentHandler(typeConverter);
                }
                else
                {
                    var headerAttribute = parameter.GetCustomAttribute<HeaderAttribute>();
                    if (headerAttribute != null)
                    {
                        argumentHandlers[parameter.Name] = new HttpHeaderArgumentHandler(typeConverter, headerAttribute.Name, headerAttribute.Values);
                    }
                    else
                    {
                        bodyParameters.Add(parameter);
                    }
                }
            }

            if (bodyParameters.Any())
            {
                if (bodyParameters.Count == 1)
                {
                    var parameter = bodyParameters.Single();
                    argumentHandlers[parameter.Name] = new DirectJsonArgumentHandler(typeConverter);
                }
                else
                {
                    foreach (var parameter in bodyParameters)
                    {
                        var jsonPropertyAttribute = parameter.GetCustomAttribute<JsonPropertyAttribute>(true);
                        var nameOverride = jsonPropertyAttribute?.PropertyName;
                        argumentHandlers[parameter.Name] = new ComposedJsonArgumentHandler(typeConverter, nameOverride);
                    }
                }
            }

            var endpoint = new HttpApiEndpoint(url, httpMethod.Method, argumentHandlers, new NullResponseHandler());
            return endpoint;
        }
    }
}