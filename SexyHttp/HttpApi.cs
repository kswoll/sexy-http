using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SexyHttp.ArgumentHandlers;
using SexyHttp.ResponseHandlers;
using SexyHttp.TypeConverters;
using SexyHttp.Urls;
using SexyHttp.Utils;

namespace SexyHttp
{
    public class HttpApi<T>
    {
        public IReadOnlyDictionary<MethodInfo, HttpApiEndpoint> Endpoints { get; }
        public ITypeConverter TypeConverter { get; }

        public HttpApi()
        {
            TypeConverter = TypeConverterAttribute.GetTypeConverter(typeof(T)) ?? new DefaultTypeConverter();

            // Create endpoints
            var endpoints = new Dictionary<MethodInfo, HttpApiEndpoint>();
            foreach (var method in typeof(T).GetMethods())
            {
                var httpMethod = method.GetHttpMethodAttribute();
                if (httpMethod != null)
                {
                    var endpoint = CreateEndpoint(method, httpMethod);
                    endpoints[method] = endpoint;
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
                var isMultipart = method.GetCustomAttribute<MultipartAttribute>() != null;

                if (isMultipart)
                {
                    foreach (var parameter in bodyParameters)
                    {
                        argumentHandlers[parameter.Name] = new MultipartArgumentHandler(typeConverter);
                    }
                }
                else
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
            }

            var returnType = method.ReturnType;
            if (!typeof(Task).IsAssignableFrom(returnType))
                throw new Exception("Methods must be async and return either Task or Task<T>");
            returnType = returnType.GetTaskType() ?? typeof(void);

            var responseHandler = returnType == typeof(void) ? 
                (IHttpResponseHandler)new NullResponseHandler() : 
                new JsonResponseHandler(typeConverter, returnType);

            var endpoint = new HttpApiEndpoint(url, httpMethod.Method, argumentHandlers, responseHandler);
            return endpoint;
        }
    }
}