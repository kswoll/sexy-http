using System.Collections.Generic;

namespace SexyHttp
{
    public class HttpApi<T>
    {
        public IReadOnlyCollection<HttpApiEndpoint> Endpoints { get; }

        public HttpApi()
        {
            // Create endpoints
            var endpoints = new List<HttpApiEndpoint>();

            foreach (var method in typeof(T).GetMethods())
            {
                
            }

            Endpoints = endpoints;
        }
    }
}