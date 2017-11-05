using System.Collections.Generic;
using System.Threading.Tasks;

namespace SexyHttp.Mocks
{
    public class MockHttpHandlerScript : IHttpHandler
    {
        private readonly Queue<IHttpHandler> script = new Queue<IHttpHandler>();

        public void Add(IHttpHandler handler)
        {
            script.Enqueue(handler);
        }

        public Task<HttpHandlerResponse> Call(HttpApiRequest request)
        {
            var handler = script.Dequeue();
            return handler.Call(request);
        }
    }
}
