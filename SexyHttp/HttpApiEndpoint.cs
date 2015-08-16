using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SexyHttp.Urls;

namespace SexyHttp
{
    public class HttpApiEndpoint
    {
        public HttpUrlDescriptor Path { get; }
        public HttpMethod Method { get; }

        public IReadOnlyDictionary<string, IHttpArgumentHandler> ArgumentHandlers { get; }
        public IHttpResponseHandler ResponseHandler { get; }
        public IReadOnlyList<HttpHeader> Headers { get; }

        public HttpApiEndpoint(
            HttpUrlDescriptor path, 
            HttpMethod method,
            Dictionary<string, IHttpArgumentHandler> argumentHandlers,
            IHttpResponseHandler responseHandler,
            IEnumerable<HttpHeader> headers)
        {
            Path = path;
            Method = method;
            ArgumentHandlers = argumentHandlers;
            ResponseHandler = responseHandler;
            Headers = headers.ToList();
        }

        public async Task<object> Call(IHttpHandler httpHandler, string baseUrl, Dictionary<string, object> arguments, IHttpApiRequestInstrumenter apiRequestInstrumenter = null)
        {
            var request = new HttpApiRequest { Url = Path.CreateUrl(baseUrl), Method = Method, Headers = Headers.ToList() };

            apiRequestInstrumenter?.InstrumentRequest(request);

            Action<Func<IHttpArgumentHandler, string, object, Task>> applyArguments = async applier =>
            {
                foreach (var item in ArgumentHandlers)
                {
                    var name = item.Key;
                    object argument;
                    if (arguments.TryGetValue(name, out argument))
                    {
                        var handler = item.Value;
                        await applier(handler, name, argument);
                    }
                }                
            };

            applyArguments(async (handler, name, argument) => await handler.ApplyArgument(request, name, argument));

            var result = await httpHandler.Call(request, async response =>
            {
                applyArguments(async (handler, name, argument) => await handler.ApplyArgument(response, name, argument));
                return await ResponseHandler.HandleResponse(response);
            });
            return result;
        }
    }
}