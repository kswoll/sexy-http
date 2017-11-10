using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SexyHttp.Mocks
{
    public class MockHttpServerScript : IDisposable
    {
        public MockHttpServer Server { get; }
        private readonly Queue<MockHttpServerHandler> script;

        public MockHttpServerScript(params MockHttpServerHandler[] handlers)
        {
            Server = new MockHttpServer(Handler);
            script = new Queue<MockHttpServerHandler>(handlers);
        }

        public MockHttpServerScript(int port, params MockHttpServerHandler[] handlers)
        {
            Server = new MockHttpServer(Handler, port);
            script = new Queue<MockHttpServerHandler>(handlers);
        }

        public MockHttpServerScript(int port, string prefix, params MockHttpServerHandler[] handlers)
        {
            Server = new MockHttpServer(Handler, port, prefix);
            script = new Queue<MockHttpServerHandler>(handlers);
        }

        private Task Handler(HttpListenerRequest request, HttpListenerResponse response)
        {
            var handler = script.Dequeue();
            return handler.Handle(request, response);
        }

        public void Dispose()
        {
            Server.Dispose();
        }
    }
}
