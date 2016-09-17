﻿using System;
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
        public IReadOnlyList<HttpHeader> Headers { get; }

        public HttpApi()
        {
            TypeConverter = TypeConverterAttribute.Combine(typeof(T), DefaultTypeConverter.Create());
            Headers = HeaderAttribute.GetHeaders(typeof(T));

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
            var endpointTypeConverter = TypeConverterAttribute.Combine(method, TypeConverter);

            // Store the additional custom headers (if any) defined on the method itself
            var headers = Headers.Concat(HeaderAttribute.GetHeaders(method));

            // Store a dictionary to store the argument handlers
            var argumentHandlers = new Dictionary<string, IHttpArgumentHandler>();

            // Get the path associated with the API as a whole
            var pathPrefix = typeof(T).GetCustomAttribute<PathAttribute>()?.Path;

            // Combine it with the path for the endpoint
            var path = httpMethod.Path;
            if (pathPrefix != null)
                path = HttpPath.Combine(pathPrefix, path);

            // Store the url to where we intend to call
            var url = HttpUrlParser.Parse(path);

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
                var typeConverter = TypeConverterAttribute.Combine(parameter, endpointTypeConverter);

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
                var isText = method.GetCustomAttribute<TextAttribute>() != null;

                Action<Func<ParameterInfo, ITypeConverter, IHttpArgumentHandler>> setBodyArgumentHandlers = factory =>
                {
                    foreach (var parameter in bodyParameters)
                    {
                        var typeConverter = TypeConverterAttribute.Combine(parameter, endpointTypeConverter);
                        argumentHandlers[parameter.Name] = factory(parameter, typeConverter);
                    }
                };

                // If the argument represents an input stream, use the respective argument handler
                if (bodyParameters.First().ParameterType == typeof(Stream) && bodyParameters.Count == 1 && !isMultipart)
                {
                    var parameter = bodyParameters.Single();
                    var typeConverter = TypeConverterAttribute.Combine(parameter, endpointTypeConverter);
                    argumentHandlers[parameter.Name] = new StreamArgumentHandler(typeConverter);
                }
                // If the argument represents an input stream, use the respective argument handler
                else if (bodyParameters.First().ParameterType == typeof(byte[]) && bodyParameters.Count == 1 && !isMultipart)
                {
                    var parameter = bodyParameters.Single();
                    var typeConverter = TypeConverterAttribute.Combine(parameter, endpointTypeConverter);
                    argumentHandlers[parameter.Name] = new ByteArrayArgumentHandler(typeConverter);
                }
                // If it's explicitly multipart or any parameter is a stream, then each parameter represent a multipart section that encapsulates the value
                else if (isMultipart || bodyParameters.Any(x => x.ParameterType == typeof(Stream)))
                {
                    setBodyArgumentHandlers((parameter, typeConverter) => new MultipartArgumentHandler(typeConverter));
                }
                else if (isForm)
                {
                    setBodyArgumentHandlers((parameter, typeConverter) => new FormArgumentHandler(typeConverter, getName(parameter)));
                }
                else if (isText)
                {
                    setBodyArgumentHandlers((parameter, typeConverter) => new StringArgumentHandler(typeConverter));
                }
                // Otherwise, we're going to serialize the request as JSON
                else
                {
                    // If there's only one body parameter and its not annotated with [Object], then we're going to serialize
                    // the argument as a raw JSON value.
                    if (bodyParameters.Count == 1 && bodyParameters.Single().GetCustomAttribute<ObjectAttribute>() == null)
                    {
                        var parameter = bodyParameters.Single();
                        var typeConverter = TypeConverterAttribute.Combine(parameter, endpointTypeConverter);
                        argumentHandlers[parameter.Name] = new DirectJsonArgumentHandler(typeConverter);
                    }
                    // Otherwise we're going to create a dynamically composed JSON object where each parameter represents a
                    // property of the composed JSON object.
                    else
                    {
                        // Foreach body parameter, create a json argument handler
                        setBodyArgumentHandlers((parameter, typeConverter) => new ComposedJsonArgumentHandler(typeConverter, getName(parameter)));
                    }
                }
            }

            var returnType = method.ReturnType;
            if (!typeof(Task).IsAssignableFrom(returnType))
                throw new Exception("Methods must be async and return either Task or Task<T>");
            returnType = returnType.GetTaskType() ?? typeof(void);

            var responseTypeConverter = TypeConverterAttribute.Combine(method.ReturnTypeCustomAttributes, endpointTypeConverter);

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
            responseHandler.TypeConverter = responseTypeConverter;
            responseHandler.ResponseType = returnType;

            var endpoint = new HttpApiEndpoint(url, httpMethod.Method, argumentHandlers, responseHandler, headers);
            return endpoint;
        }
    }
}