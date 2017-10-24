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

        public Task<object> Call(IHttpHandler httpHandler, string baseUrl, Dictionary<string, object> arguments, HttpApiInstrumenter apiInstrumenter = null)
        {
            return Call(httpHandler, baseUrl, new HttpApiArguments(arguments), apiInstrumenter);
        }

        public async Task<object> Call(IHttpHandler httpHandler, string baseUrl, HttpApiArguments arguments, HttpApiInstrumenter apiInstrumenter = null)
        {
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

            HttpApiRequest GetRequest()
            {
                var request = new HttpApiRequest { Url = Url.CreateUrl(baseUrl), Method = HttpMethod, Headers = Headers.ToList() };

                ApplyArguments(async (handler, name, argument) => await handler.ApplyArgument(request, name, argument));

                return request;
            }

            async Task<HttpHandlerResponse> GetResponse(HttpApiRequest request)
            {
                return await httpHandler.Call(request);
            }

            async Task<object> GetResult(HttpApiRequest request, HttpHandlerResponse response)
            {
                ApplyArguments(async (handler, name, argument) => await handler.ApplyArgument(response.ApiResponse, name, argument));
                return await ResponseHandler.HandleResponse(request, response.ApiResponse);
            }

            IHttpApiInstrumentation instrumentation = new DefaultHttpApiInstrumentation(GetRequest, GetResponse, GetResult);
            if (apiInstrumenter != null)
                instrumentation = apiInstrumenter(this, arguments, instrumentation);

            return await MakeCall(instrumentation);
        }

        private async Task<object> MakeCall(IHttpApiInstrumentation instrumentation)
        {
            var requests = instrumentation.GetRequests();
            object result = null;
            foreach (var request in requests)
            {
                var response = await instrumentation.GetResponse(request);
                var lastResult = result;
                result = await instrumentation.GetResult(request, response);
                result = instrumentation.AggregateResult(request, response, lastResult, result);
            }
            return result;
        }

        private class DefaultHttpApiInstrumentation : IHttpApiInstrumentation
        {
            private readonly Func<HttpApiRequest> getRequest;
            private readonly Func<HttpApiRequest, Task<HttpHandlerResponse>> getResponse;
            private readonly Func<HttpApiRequest, HttpHandlerResponse, Task<object>> getResult;

            public DefaultHttpApiInstrumentation(Func<HttpApiRequest> getRequest, Func<HttpApiRequest, Task<HttpHandlerResponse>> getResponse, Func<HttpApiRequest, HttpHandlerResponse, Task<object>> getResult)
            {
                this.getRequest = getRequest;
                this.getResponse = getResponse;
                this.getResult = getResult;
            }

            public IEnumerable<HttpApiRequest> GetRequests()
            {
                yield return getRequest();
            }

            public Task<HttpHandlerResponse> GetResponse(HttpApiRequest request)
            {
                return getResponse(request);
            }

            public Task<object> GetResult(HttpApiRequest request, HttpHandlerResponse response)
            {
                return getResult(request, response);
            }

            public object AggregateResult(HttpApiRequest request, HttpHandlerResponse response, object lastResult, object result)
            {
                return result;
            }
        }
    }
}