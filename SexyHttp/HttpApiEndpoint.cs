using System.Collections.Generic;
using System.Threading.Tasks;

namespace SexyHttp
{
    public class HttpApiEndpoint
    {
        public string Path { get; }

        private readonly IHttpHandler httpHandler;
        private readonly IHttpHeadersProvider headersProvider;
        private readonly IDictionary<string, IHttpArgumentHandler> argumentHandlers;
        private readonly IHttpResponseHandler responseHandler;

        public HttpApiEndpoint(
            string path, 
            IHttpHandler httpHandler, 
            IHttpHeadersProvider headersProvider,
            IDictionary<string, IHttpArgumentHandler> argumentHandlers,
            IHttpResponseHandler responseHandler)
        {
            Path = path;

            this.httpHandler = httpHandler;
            this.headersProvider = headersProvider;
            this.argumentHandlers = argumentHandlers;
            this.responseHandler = responseHandler;
        }

        public async Task<object> Call(string baseUrl, Dictionary<string, object> arguments)
        {
            var url = baseUrl + "/" + Path;
            var request = new HttpApiRequest { Url = url, Headers = new List<HttpHeader>() };

            headersProvider.ProvideHeaders(request);

            foreach (var item in argumentHandlers)
            {
                var name = item.Key;
                var argument = arguments[name];
                var handler = item.Value;
                handler.ApplyArgument(request, name, argument);
            }

            var response = await httpHandler.Call(request);
            var result = responseHandler.HandleResponse(response);
            return result;
        }
    }
}