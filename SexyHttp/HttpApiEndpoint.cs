﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SexyHttp.Urls;

namespace SexyHttp
{
    public class HttpApiEndpoint
    {
        public HttpUrlDescriptor Url { get; }
        public HttpMethod Method { get; }

        public IReadOnlyDictionary<string, IHttpArgumentHandler> ArgumentHandlers { get; }
        public IHttpResponseHandler ResponseHandler { get; }
        public IReadOnlyList<HttpHeader> Headers { get; }

        public HttpApiEndpoint(
            HttpUrlDescriptor url,
            HttpMethod method,
            Dictionary<string, IHttpArgumentHandler> argumentHandlers,
            IHttpResponseHandler responseHandler,
            IEnumerable<HttpHeader> headers)
        {
            Url = url;
            Method = method;
            ArgumentHandlers = argumentHandlers;
            ResponseHandler = responseHandler;
            Headers = headers.ToList();
        }

        public async Task<object> Call(IHttpHandler httpHandler, string baseUrl, Dictionary<string, object> arguments, HttpApiInstrumenter apiInstrumenter = null)
        {
            var request = new HttpApiRequest { Url = Url.CreateUrl(baseUrl), Method = Method, Headers = Headers.ToList() };

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

            Func<HttpApiRequest, Task<HttpApiResponse>> call = async apiRequest => await httpHandler.Call(apiRequest);

            HttpApiResponse response;
            if (apiInstrumenter != null)
                response = await apiInstrumenter(request, call);
            else
                response = await call(request);

            applyArguments(async (handler, name, argument) => await handler.ApplyArgument(response, name, argument));
            return await ResponseHandler.HandleResponse(request, response);
        }
    }
}