using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SexyHttp.Urls;

namespace SexyHttp
{
    public class HttpApiEndpoint
    {
        public HttpUrlDescriptor Path { get; }
        public HttpMethod Method { get; }

        private readonly IDictionary<string, IHttpArgumentHandler> argumentHandlers;
        private readonly IHttpResponseHandler responseHandler;

        public HttpApiEndpoint(
            HttpUrlDescriptor path, 
            HttpMethod method,
            IDictionary<string, IHttpArgumentHandler> argumentHandlers,
            IHttpResponseHandler responseHandler)
        {
            Path = path;
            Method = method;

            this.argumentHandlers = argumentHandlers;
            this.responseHandler = responseHandler;
        }

        public async Task<object> Call(IHttpHandler httpHandler, string baseUrl, Dictionary<string, object> arguments, IHttpApiRequestInstrumenter apiRequestInstrumenter = null)
        {
            var request = new HttpApiRequest { Url = Path.CreateUrl(baseUrl), Method = Method, Headers = new List<HttpHeader>() };

            apiRequestInstrumenter?.InstrumentRequest(request);

            Action<Func<IHttpArgumentHandler, string, object, Task>> applyArguments = async applier =>
            {
                foreach (var item in argumentHandlers)
                {
                    var name = item.Key;
                    object argument;
                    if (!arguments.TryGetValue(name, out argument))
                        throw new Exception($"The argument {name} was not found in the request.");
                    var handler = item.Value;
                    await applier(handler, name, argument);
                }                
            };

            applyArguments(async (handler, name, argument) => await handler.ApplyArgument(request, name, argument));

            var result = await httpHandler.Call(request, async response =>
            {
                applyArguments(async (handler, name, argument) => await handler.ApplyArgument(response, name, argument));
                return await responseHandler.HandleResponse(response);
            });
            return result;
        }
    }
}