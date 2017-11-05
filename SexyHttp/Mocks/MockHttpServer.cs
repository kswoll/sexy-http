﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SexyHttp.HttpBodies;
using SexyHttp.Tests.Utils;
using SexyHttp.Utils;

namespace SexyHttp.Mocks
{
    public class MockHttpServer : IDisposable
    {
        private readonly HttpListener listener = new HttpListener();
        private readonly object locker = new object();
        private bool isRunning;

        public MockHttpServer(Func<HttpListenerRequest, HttpListenerResponse, Task> handler)
        {
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

        private static async Task<JToken> ReadJson(HttpListenerRequest request)
        {
            using (var reader = new StreamReader(request.InputStream))
            {
                var inputString = await reader.ReadToEndAsync();
                var jsonInput = JToken.Parse(inputString);
                return jsonInput;
            }
        }

        private static async Task<string> ReadString(HttpListenerRequest request)
        {
            using (var reader = new StreamReader(request.InputStream))
            {
                var inputString = await reader.ReadToEndAsync();
                return inputString;
            }
        }

        private static async Task WriteString(HttpListenerResponse response, string s)
        {
            response.Headers.Add("Content-Type", "text/plain");
            var buffer = Encoding.UTF8.GetBytes(s);
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }

        private static async Task WriteJson(HttpListenerResponse response, JToken json)
        {
            response.Headers.Add("Content-Type", "application/json");
            var s = json.ToString(Formatting.Indented);
            var buffer = Encoding.UTF8.GetBytes(s);
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }

        private static async Task WriteByteArray(HttpListenerResponse response, byte[] data)
        {
            response.Headers.Add("Content-Type", "application/octet-stream");
            await response.OutputStream.WriteAsync(data, 0, data.Length);
            response.OutputStream.Close();
        }

        private static async Task WriteForm(HttpListenerResponse response, FormHttpBody form)
        {
            response.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            var content = new FormUrlEncodedContent(form.Values);
            var data = await content.ReadAsStreamAsync();
            await data.CopyToAsync(response.OutputStream);
            response.OutputStream.Close();
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
            return new MockHttpServer(async (request, response) =>
            {
                var token = await jsonHandler(request);
                await WriteJson(response, token);
            });
        }

        public static MockHttpServer ReturnJson(JToken json)
        {
            return new MockHttpServer(async (request, response) =>
            {
                await WriteJson(response, json);
            });
        }

        public static MockHttpServer ReturnJson(Func<HttpListenerRequest, JToken> jsonHandler)
        {
            return new MockHttpServer(async (request, response) =>
            {
                var json = jsonHandler(request);
                await WriteJson(response, json);
            });
        }

        public static MockHttpServer ReturnByteArray(Func<HttpListenerRequest, byte[]> handler)
        {
            return new MockHttpServer(async (request, response) =>
            {
                var token = handler(request);
                await WriteByteArray(response, token);
            });
        }

        public static MockHttpServer String(Func<string, string> handler)
        {
            return new MockHttpServer(async (request, response) =>
            {
                var s = await ReadString(request);
                var output = handler(s);
                await WriteString(response, output);
            });
        }

        public static MockHttpServer Json(Func<JToken, JToken> jsonHandler)
        {
            return new MockHttpServer(async (request, response) =>
            {
                var jsonInput = await ReadJson(request);
                var jsonOutput = jsonHandler(jsonInput);
                await WriteJson(response, jsonOutput);
            });
        }

        public static MockHttpServer Json(Action<JToken> handler)
        {
            return new MockHttpServer(async (request, response) =>
            {
                var jsonInput = await ReadJson(request);
                handler(jsonInput);
                response.StatusCode = (int)HttpStatusCode.NoContent;
                response.OutputStream.Close();
            });
        }

        public static MockHttpServer PostMultipartReturnJson(Func<MultipartHttpBody, Task<JToken>> handler)
        {
            return new MockHttpServer(async (request, response) =>
            {
                var content = MultipartParser.ParseMultipart(request);
                var json = await handler(content);
                await WriteJson(response, json);
            });
        }

        public static MockHttpServer PostMultipartReturnByteArray(Func<MultipartHttpBody, Task<byte[]>> handler)
        {
            return new MockHttpServer(async (request, response) =>
            {
                var content = MultipartParser.ParseMultipart(request);
                var data = await handler(content);
                await WriteByteArray(response, data);
            });
        }

        public static MockHttpServer PostStreamReturnByteArray(Func<Stream, Task<byte[]>> handler)
        {
            return new MockHttpServer(async (request, response) =>
            {
                var data = await handler(request.InputStream);
                await WriteByteArray(response, data);
            });
        }

        public static MockHttpServer PostByteArrayReturnByteArray(Func<byte[], Task<byte[]>> handler)
        {
            return new MockHttpServer(async (request, response) =>
            {
                var data = await handler(await request.InputStream.ReadToEndAsync());
                await WriteByteArray(response, data);
            });
        }

        public static MockHttpServer PostMultipartStreamReturnJson(Func<MultipartHttpBody, Task<JToken>> handler)
        {
            return new MockHttpServer(async (request, response) =>
            {
                var content = MultipartParser.ParseMultipart(request);
                var data = await handler(content);
                await WriteJson(response, data);
            });
        }

        public static MockHttpServer PostFormReturnJson(Func<FormHttpBody, JToken> handler)
        {
            return PostFormReturnJson(x => Task.FromResult(handler(x)));
        }

        public static MockHttpServer PostFormReturnJson(Func<FormHttpBody, Task<JToken>> handler)
        {
            return new MockHttpServer(async (request, response) =>
            {
                var content = FormParser.ParseForm(request.InputStream);
                var result = await handler(content);
                await WriteJson(response, result);
            });
        }

        public static MockHttpServer PostFormReturnForm(Func<FormHttpBody, Task<FormHttpBody>> handler)
        {
            return new MockHttpServer(async (request, response) =>
            {
                var content = FormParser.ParseForm(request.InputStream);
                var result = await handler(content);
                await WriteForm(response, result);
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