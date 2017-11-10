using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SexyHttp.Utils;

namespace SexyHttp.Mocks
{
    public class MockHttpServerHandler
    {
        public HttpListenerRequest Request { get; set; }

        private Func<MockHttpServerHandlerContext, Task> Handler { get; }
        private readonly HttpStatusCode statusCode;

        public MockHttpServerHandler(HttpStatusCode statusCode = HttpStatusCode.OK)
            : this(context => TaskConstants.Completed, statusCode)
        {
        }

        public MockHttpServerHandler(object response, HttpStatusCode statusCode = HttpStatusCode.OK)
            : this(JToken.FromObject(response), statusCode)
        {
        }

        public MockHttpServerHandler(JToken token, HttpStatusCode statusCode = HttpStatusCode.OK)
            : this(context => context.WriteJson(token), statusCode)
        {
        }

        public MockHttpServerHandler(Func<MockHttpServerHandlerContext, Task> handler, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            Handler = handler;
            this.statusCode = statusCode;
        }

        public async Task Handle(HttpListenerRequest request, HttpListenerResponse response)
        {
            Request = request;
            response.StatusCode = (int)statusCode;
            var context = new MockHttpServerHandlerContext(request, response);
            await Handler(context);
            response.OutputStream.Close();
        }
    }
}
