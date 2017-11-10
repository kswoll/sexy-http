using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SexyHttp.HttpBodies;
using SexyHttp.Utils;

namespace SexyHttp.Mocks
{
    public class MockHttpServer : IDisposable
    {
        private readonly HttpListener listener = new HttpListener();
        private readonly object locker = new object();
        private bool isRunning;

        public MockHttpServer(Func<MockHttpServerHandlerContext, Task> handler, int port = 8844, string prefix = "")
            : this((request, response) => new MockHttpServerHandler(handler).Handle(request, response), port, prefix)
        {
        }

        public MockHttpServer(Func<HttpListenerRequest, HttpListenerResponse, Task> handler, int port = 8844, string prefix = "")
        {
            listener.Prefixes.Add($"http://+:{port}{prefix}/");

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
                    throw new Exception($"Please run: netsh http add urlacl url=http://+:{port}/ sddl=D:(A;;GX;;;WD)");
                }
                throw;
            }
            isRunning = true;
#pragma warning disable 4014
            Task.Run(async () =>
#pragma warning restore 4014
            {
                var localIsRunning = isRunning;
                while (localIsRunning)
                {
                    var context = await listener.GetContextAsync();
                    try
                    {
                        await handler(context.Request, context.Response);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Dispose();
                        break;
                    }
                    lock (locker)
                    {
                        localIsRunning = isRunning;
                    }
                }
            });
        }

        public static MockHttpServer Null(Action<HttpListenerRequest> handler)
        {
            return new MockHttpServer((request, response) =>
            {
                handler(request);
                response.OutputStream.Close();
                return TaskConstants.Completed;
            });
        }

        public static MockHttpServer Raw(Action<HttpListenerRequest, HttpListenerResponse> handler)
        {
            return new MockHttpServer((request, response) =>
            {
                handler(request, response);
                response.OutputStream.Close();
                return TaskConstants.Completed;
            });
        }

        public static MockHttpServer ReturnJson(Func<HttpListenerRequest, Task<JToken>> jsonHandler)
        {
            return new MockHttpServer(async context =>
            {
                var token = await jsonHandler(context.Request);
                await context.WriteJson(token);
            });
        }

        public static MockHttpServer ReturnJson(JToken json)
        {
            return new MockHttpServer(async context =>
            {
                await context.WriteJson(json);
            });
        }

        public static MockHttpServer ReturnJson(Func<HttpListenerRequest, JToken> jsonHandler)
        {
            return new MockHttpServer(async context =>
            {
                var json = jsonHandler(context.Request);
                await context.WriteJson(json);
            });
        }

        public static MockHttpServer ReturnByteArray(Func<HttpListenerRequest, byte[]> handler)
        {
            return new MockHttpServer(async context =>
            {
                var token = handler(context.Request);
                await context.WriteByteArray(token);
            });
        }

        public static MockHttpServer String(Func<string, string> handler)
        {
            return new MockHttpServer(async context =>
            {
                var s = await context.ReadString();
                var output = handler(s);
                await context.WriteString(output);
            });
        }

        public static MockHttpServer Json(Func<JToken, JToken> jsonHandler)
        {
            return new MockHttpServer(async context =>
            {
                var jsonInput = await context.ReadJson();
                var jsonOutput = jsonHandler(jsonInput);
                await context.WriteJson(jsonOutput);
            });
        }

        public static MockHttpServer Json(Action<JToken> handler)
        {
            return new MockHttpServer(async context =>
            {
                var jsonInput = await context.ReadJson();
                handler(jsonInput);
                context.Response.StatusCode = (int)HttpStatusCode.NoContent;
            });
        }

        public static MockHttpServer PostMultipartReturnJson(Func<MultipartHttpBody, Task<JToken>> handler)
        {
            return new MockHttpServer(async context =>
            {
                var content = context.ReadMultipart();
                var json = await handler(content);
                await context.WriteJson(json);
            });
        }

        public static MockHttpServer PostMultipartReturnByteArray(Func<MultipartHttpBody, Task<byte[]>> handler)
        {
            return new MockHttpServer(async context =>
            {
                var content = context.ReadMultipart();
                var data = await handler(content);
                await context.WriteByteArray(data);
            });
        }

        public static MockHttpServer PostStreamReturnByteArray(Func<Stream, Task<byte[]>> handler)
        {
            return new MockHttpServer(async context =>
            {
                var data = await handler(context.Request.InputStream);
                await context.WriteByteArray(data);
            });
        }

        public static MockHttpServer PostByteArrayReturnByteArray(Func<byte[], Task<byte[]>> handler)
        {
            return new MockHttpServer(async context =>
            {
                var data = await handler(await context.ReadByteArray());
                await context.WriteByteArray(data);
            });
        }

        public static MockHttpServer PostMultipartStreamReturnJson(Func<MultipartHttpBody, Task<JToken>> handler)
        {
            return new MockHttpServer(async context =>
            {
                var content = context.ReadMultipart();
                var data = await handler(content);
                await context.WriteJson(data);
            });
        }

        public static MockHttpServer PostFormReturnJson(Func<FormHttpBody, JToken> handler)
        {
            return PostFormReturnJson(x => Task.FromResult(handler(x)));
        }

        public static MockHttpServer PostFormReturnJson(Func<FormHttpBody, Task<JToken>> handler)
        {
            return new MockHttpServer(async context =>
            {
                var content = context.ReadForm();
                var result = await handler(content);
                await context.WriteJson(result);
            });
        }

        public static MockHttpServer PostFormReturnForm(Func<FormHttpBody, Task<FormHttpBody>> handler)
        {
            return new MockHttpServer(async context =>
            {
                var content = context.ReadForm();
                var result = await handler(content);
                await context.WriteForm(result);
            });
        }

        public static MockHttpServer PostFormReturnForm(Func<FormHttpBody, FormHttpBody> handler)
        {
            return PostFormReturnForm(x => Task.FromResult(handler(x)));
        }

        public void Dispose()
        {
            lock (locker)
            {
                isRunning = false;
            }
            if (listener.IsListening)
                listener.Stop();
        }
    }
}