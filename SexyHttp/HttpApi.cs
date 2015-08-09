using System;
using System.Collections.Generic;
using System.IO;
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
            var explicitTypeConverter = TypeConverterAttribute.GetTypeConverter(typeof(T));
            TypeConverter = explicitTypeConverter != null ? new CombinedTypeConverter(explicitTypeConverter, new DefaultTypeConverter()) : new DefaultTypeConverter();

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
                // If the parameter is Func<Stream, Task> then it should be invoked with the response stream for custom handling.
                else if (parameter.ParameterType == typeof(Func<Stream, Task>))
                {
                    argumentHandlers[parameter.Name] = new StreamResponseArgumentHandler(typeConverter);
                }
                else if (parameter.ParameterType == typeof(Action<HttpApiRequest>))
                {
                    argumentHandlers[parameter.Name] = new HttpApiRequestArgumentHandler(typeConverter);
                }
                else if (parameter.ParameterType == typeof(HttpBody))
                {
                    argumentHandlers[parameter.Name] = new HttpBodyArgumentHandler(typeConverter);
                }
                else
                {
                    bodyParameters.Add(parameter);
                }
            }

            // You can override the name associated with the parameter by using either JsonPropertyAttribute or 
            // NameAttribute.  We provide a NameAttribute to avoid forcing you to use JsonPropertyAttribute when
            // not dealing with JSON.
            Func<ParameterInfo, string> getName = parameter =>
            {
                var jsonPropertyAttribute = parameter.GetCustomAttribute<JsonPropertyAttribute>(true);
                var nameAttribute = parameter.GetCustomAttribute<NameAttribute>(true);

                return nameAttribute?.Value ?? jsonPropertyAttribute?.PropertyName ?? parameter.Name;
            };

            // If there is data for a body, then create a handler to provide a body
            if (bodyParameters.Any())
            {
                var isMultipart = method.GetCustomAttribute<MultipartAttribute>() != null;
                var isForm = method.GetCustomAttribute<FormAttribute>() != null;
                var isRaw = method.GetCustomAttribute<RawAttribute>() != null;

                // If the argument represents an input stream, use the respective argument handler
                if (bodyParameters.First().ParameterType == typeof(Stream) && bodyParameters.Count == 1)
                {
                    var parameter = bodyParameters.Single();
                    argumentHandlers[parameter.Name] = new StreamArgumentHandler(typeConverter);
                }
                // If it's explicitly multipart or any parameter is a stream, then each parameter represent a multipart section that encapsulates the value
                else if (isMultipart || bodyParameters.Any(x => x.ParameterType == typeof(Stream)))
                {
                    foreach (var parameter in bodyParameters)
                    {
                        argumentHandlers[parameter.Name] = new MultipartArgumentHandler(typeConverter);
                    }
                }
                else if (isForm)
                {
                    foreach (var parameter in bodyParameters)
                    {
                        argumentHandlers[parameter.Name] = new FormArgumentHandler(typeConverter, getName(parameter));
                    }
                }
                else if (isRaw)
                {
                    foreach (var parameter in bodyParameters)
                    {
                        argumentHandlers[parameter.Name] = new RawArgumentHandler(typeConverter);
                    }
                }
                // Otherwise, we're going to serialize the request as JSON
                else
                {
                    // If there's only one body parameter, then we're going to serialize the argument as a raw JSON value. 
                    // (Unless it's decorated with [Object] in which case we force serialization as an object below)
                    if (bodyParameters.Count == 1 && bodyParameters.Single().GetCustomAttribute<ObjectAttribute>() == null)
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
                            argumentHandlers[parameter.Name] = new ComposedJsonArgumentHandler(typeConverter, getName(parameter));
                        }
                    }                    
                }
            }

            var returnType = method.ReturnType;
            if (!typeof(Task).IsAssignableFrom(returnType))
                throw new Exception("Methods must be async and return either Task or Task<T>");
            returnType = returnType.GetTaskType() ?? typeof(void);

            IHttpResponseHandler responseHandler;
            if (returnType == typeof(void))
                responseHandler = new NullResponseHandler();
            else if (returnType == typeof(byte[]))
                responseHandler = new ByteArrayResponseHandler();
            else if (returnType == typeof(HttpApiResponse))
                responseHandler = new HttpApiResponseResponseHandler();
            else if (returnType == typeof(HttpBody))
                responseHandler = new HttpBodyResponseHandler();
            else
                responseHandler = new ContentTypeBasedResponseHandler();
            responseHandler.TypeConverter = typeConverter;
            responseHandler.ResponseType = returnType;

            var endpoint = new HttpApiEndpoint(url, httpMethod.Method, argumentHandlers, responseHandler);
            return endpoint;
        }
    }
}