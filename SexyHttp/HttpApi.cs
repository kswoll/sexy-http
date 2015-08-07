using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SexyHttp.ArgumentHandlers;
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
            // Store a variable to store the type converter to be used for this endpoint
            var typeConverter = TypeConverterAttribute.Combine(method, TypeConverter);

            // Store a dictionary to store the argument handlers
            var argumentHandlers = new Dictionary<string, IHttpArgumentHandler>();

            // Store the url to where we intend to call
            var url = HttpUrlParser.Parse(httpMethod.Path);

            // Create a hash set so we can quickly deterine which parameters appy to the path
            var pathParameters = new HashSet<string>(url.PathParts.OfType<VariableHttpPathPart>().Select(x => x.Key));

            // Create a hash set so we can quickly determine which parameters apply to the query string
            var queryParameters = new HashSet<string>(url.QueryParts.Select(x => x.Value).OfType<VariableHttpPathPart>().Select(x => x.Key));

            // Store a list that aren't for anything else and thus must be consigned to the body
            var bodyParameters = new List<ParameterInfo>();

            // Iterate through the parameters and figure out which argument handler to use for each
            foreach (var parameter in method.GetParameters())
            {
                // If the parameter overrides the type converter, we'll overwrite the typeConverter variable
                typeConverter = TypeConverterAttribute.Combine(parameter, typeConverter);

                // Store headerAttribute for later
                var headerAttribute = parameter.GetCustomAttribute<HeaderAttribute>();

                // If this is a path paraeter, then create a respective argument handler
                if (pathParameters.Contains(parameter.Name))
                {
                    argumentHandlers[parameter.Name] = new PathArgumentHandler(typeConverter);
                }
                // Otherwise, if it's a query string parameter, create an argument handler for that
                else if (queryParameters.Contains(parameter.Name))
                {
                    argumentHandlers[parameter.Name] = new QueryArgumentHandler(typeConverter);
                }
                // See if the parameter represents a header.  If it does, create the respective argument handler.
                else if (headerAttribute != null)
                {
                    argumentHandlers[parameter.Name] = new HttpHeaderArgumentHandler(typeConverter, headerAttribute.Name, headerAttribute.Values);
                }
                else if (parameter.ParameterType == typeof(Func<Stream>))
                {
                    argumentHandlers[parameter.Name] = new StreamArgumentHandler(typeConverter);
                }
                else
                {
                    bodyParameters.Add(parameter);
                }
            }

            // If there is data for a body, then create a handler to provide a body
            if (bodyParameters.Any())
            {
                var isMultipart = method.GetCustomAttribute<MultipartAttribute>() != null;

                // If it's explicitly multipart, then each parameter represent a multipart section that encapsulates the value
                if (isMultipart)
                {
                    foreach (var parameter in bodyParameters)
                    {
                        argumentHandlers[parameter.Name] = new MultipartArgumentHandler(typeConverter);
                    }
                }
                // Otherwise, we're going to serialize the request as JSON
                else
                {
                    // If there's only one body parameter, then we're going to serialize the argument as a raw JSON value
                    if (bodyParameters.Count == 1)
                    {
                        var parameter = bodyParameters.Single();
                        argumentHandlers[parameter.Name] = new DirectJsonArgumentHandler(typeConverter);
                    }
                    // Otherwise we're going to create a dynamically composed JSON object where each parameter represents a 
                    // proprety the composed JSON object.
                    else
                    {
                        // Foreach body parameter, create a json argument handler
                        foreach (var parameter in bodyParameters)
                        {
                            // You can override the property name of the composed JSON object by using JSO
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

            var responseHandler = typeConverter.ConvertTo<IHttpResponseHandler>(returnType);
            responseHandler.TypeConverter = typeConverter;

            var endpoint = new HttpApiEndpoint(url, httpMethod.Method, argumentHandlers, responseHandler);
            return endpoint;
        }
    }
}