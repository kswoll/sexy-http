using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SexyHttp.Tests
{
    public class MockHttpServer : IDisposable
    {
        private readonly HttpListener listener = new HttpListener();
        private readonly Func<HttpListenerRequest, HttpListenerResponse, Task> handler;
        private bool isRunning;

        public MockHttpServer(Func<HttpListenerRequest, JToken> jsonHandler) : this(async (request, response) =>
            {
                response.Headers.Add("Content-Type", "application/json");
                var token = jsonHandler(request);
                var s = token.ToString(Formatting.Indented);
                var buffer = Encoding.UTF8.GetBytes(s);
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            })
        {
        }

        public MockHttpServer(Func<HttpListenerRequest, HttpListenerResponse, Task> handler)
        {
            this.handler = handler;
            listener.Prefixes.Add("http://+:8844/");

            try
            {
                listener.Start();
            }
            catch (HttpListenerException e)
            {
                if (e.ErrorCode == 5)
                {
                    // Make sure to run:
                    // netsh http add urlacl url=http://+:80/PlanGrid sddl=D:(A;;GX;;;WD)
                    // In an elevated Window to grant rights to non-admin users to listen on this url

                    throw new Exception("Please run: netsh http add urlacl url=http://+:8844/ sddl=D:(A;;GX;;;WD)");
                }
                throw;
            }
            isRunning = true;
#pragma warning disable 4014
            Task.Run(async () =>
#pragma warning restore 4014
            {
                while (isRunning)
                {
                    var context = await listener.GetContextAsync();
                    await handler(context.Request, context.Response);
                }
            });
        }

        public void Dispose()
        {
            isRunning = false;
            if (listener.IsListening)
                listener.Stop();
        }
    }
}