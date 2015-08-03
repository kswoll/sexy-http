using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SexyHttp
{
    public class HttpApiEndpoint
    {
        public HttpPath Path { get; }
        public HttpMethod Method { get; }

        private readonly IDictionary<string, IHttpArgumentHandler> argumentHandlers;
        private readonly IHttpResponseHandler responseHandler;

        public HttpApiEndpoint(
            string path, 
            HttpMethod method,
            IDictionary<string, IHttpArgumentHandler> argumentHandlers,
            IHttpResponseHandler responseHandler)
        {
            Path = path;
            Method = method;

            this.argumentHandlers = argumentHandlers;
            this.responseHandler = responseHandler;
        }

        public async Task<object> Call(IHttpHandler httpHandler, IHttpHeadersProvider headersProvider, string baseUrl, Dictionary<string, object> arguments)
        {
            var url = baseUrl + "/" + Path.ToString(arguments);
            var request = new HttpApiRequest { Url = url, Method = Method, Headers = new List<HttpHeader>() };

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