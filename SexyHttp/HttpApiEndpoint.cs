using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using SexyHttp.Urls;

namespace SexyHttp
{
    /// <summary>
    /// An instance of this class is generated for each of the method definitions in your API interface.
    /// It is what is responsible for translating the incoming C# types into values in the HTTP request,
    /// and translating the values in the HTTP response into the outgoing C# types.
    /// </summary>
    public class HttpApiEndpoint
    {
        public MethodInfo Method { get; }
        public HttpUrlDescriptor Url { get; }
        public HttpMethod HttpMethod { get; }

        public IReadOnlyDictionary<string, IHttpArgumentHandler> ArgumentHandlers { get; }
        public IHttpResponseHandler ResponseHandler { get; }
        public IReadOnlyList<HttpHeader> Headers { get; }

        public HttpApiEndpoint(
            MethodInfo method,
            HttpUrlDescriptor url,
            HttpMethod httpMethod,
            Dictionary<string, IHttpArgumentHandler> argumentHandlers,
            IHttpResponseHandler responseHandler,
            IEnumerable<HttpHeader> headers, bool isAsync = true)
        {
            Method = method;
            Url = url;
            HttpMethod = httpMethod;
            ArgumentHandlers = argumentHandlers;
            ResponseHandler = responseHandler;
            Headers = headers.ToList();
        }

        public async Task<object> Call(IHttpHandler httpHandler, string baseUrl, Dictionary<string, object> arguments, HttpApiInstrumenter apiInstrumenter = null)
        {
            var request = new HttpApiRequest { Url = Url.CreateUrl(baseUrl), Method = HttpMethod, Headers = Headers.ToList() };

            async void ApplyArguments(Func<IHttpArgumentHandler, string, object, Task> applier)
            {
                foreach (var item in ArgumentHandlers)
                {
                    var name = item.Key;
                    if (arguments.TryGetValue(name, out var argument))
                    {
                        var handler = item.Value;
                        await applier(handler, name, argument);
                    }
                }
            }

            ApplyArguments(async (handler, name, argument) => await handler.ApplyArgument(request, name, argument));

            async Task<HttpHandlerResponse> MakeCall(HttpApiRequest apiRequest) => await httpHandler.Call(apiRequest);

            HttpHandlerResponse response;
            if (apiInstrumenter != null)
                response = await apiInstrumenter(this, request, MakeCall);
            else
                response = await MakeCall(request);

            ApplyArguments(async (handler, name, argument) => await handler.ApplyArgument(response.ApiResponse, name, argument));
            return await ResponseHandler.HandleResponse(request, response.ApiResponse);
        }
    }
}