using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;
using SexyHttp.HttpBodies;

namespace SexyHttp.Tests
{
    public class MultipartParser
    {
        public static MultipartHttpBody ParseMultipart(HttpListenerRequest request)
        {
            var extraData = request.ContentType
                .Split(';')
                .Skip(1)
                .Select(x => x.Trim().Split('='))
                .ToDictionary(x => x[0], x => x[1].StartsWith("\"") ? x[1].Substring(1, x[1].Length - 2) : x[1]);

            var boundary = "--" + extraData["boundary"];
            var input = new MemoryStream();
            var position = 0L;
            request.InputStream.CopyTo(input);
            var buffer = input.ToArray();
            var result = new MultipartHttpBody();

//            var s2 = new StreamReader(new MemoryStream(buffer)).ReadToEnd();
            var boundaryBytes = Encoding.UTF8.GetBytes(boundary);

            var boundaryLine = ReadLine(buffer, ref position);
            if (boundaryLine != boundary)
                throw new Exception($"Expected boundary but found: {boundaryLine}");

            while (true)
            {
                string name = null;
                string fileName = null;
                string contentType = null;
                for (var line = ReadLine(buffer, ref position); line != ""; line = ReadLine(buffer, ref position))
                {
                    int colonIndex = line.IndexOf(':');
                    var headerName = line.Substring(0, colonIndex);
                    var headerValue = line.Substring(colonIndex + 2);

                    switch (headerName)
                    {
                        case "Content-Disposition":
                        {
                            var header = ContentDispositionHeaderValue.Parse(headerValue);
                            name = header.Name;
                            fileName = header.FileName;
                            break;
                        }
                        case "Content-Type":
                        {
                            var header = MediaTypeHeaderValue.Parse(headerValue);
                            contentType = header.MediaType;
                            break;
                        }
                    }
                }

                var endBoundary = IndexOf(buffer, position, boundaryBytes);
                var dataBuffer = ReadBuffer(buffer, ref position, endBoundary - position);
                Func<string> getText = () =>
                {
                    var s = Encoding.UTF8.GetString(dataBuffer);
//                    s = s.Substring(0, s.Length - 2);
                    return s;
                };
                HttpBody body;
                switch (contentType)
                {
                    case "text/plain":
                        body = new StringHttpBody(getText());
                        break;
                    case "application/json":
                        body = new JsonHttpBody(JToken.Parse(getText()));
                        break;
                    case "application/octet-stream":
                        body = new ByteArrayHttpBody { Data = dataBuffer };
                        break;
                    default:
                        throw new Exception($"Unsupported media type: {contentType}");
                }
                result.Data[name] = new MultipartData { FileName = fileName, Body = body };

                var endBoundaryLine = ReadLine(buffer, ref position);
                if (endBoundaryLine == boundaryLine + "--")
                    break;
                if (endBoundaryLine != boundaryLine)
                    throw new Exception($"Expected ending boundary but found: {boundaryLine}");
            }
            return result;
        }

        private static byte[] ReadBuffer(byte[] bytes, ref long position, long length)
        {
            var dataBuffer = new byte[length - 2];
            Array.Copy(bytes, position, dataBuffer, 0, dataBuffer.Length);
            position += dataBuffer.Length + 2;
            return dataBuffer;
        }

        private static string ReadLine(byte[] bytes, ref long position)
        {
            var buffer = new List<byte>();
            for (; position < bytes.Length; position++)
            {
                var current = bytes[position];
                if (current == '\r')
                {
                    position += 2;
                    break;
                }
                buffer.Add(current);
            }
            return Encoding.UTF8.GetString(buffer.ToArray());
        }

        private static long IndexOf(byte[] bytes, long position, byte[] search)
        {
            for (; position < bytes.Length; position++)
            {
                for (var i = 0; i < search.Length; i++)
                {
                    var searchByte = search[i];
                    var bytesByte = bytes[position + i];
                    if (bytesByte != searchByte)
                        goto notFound;
                }

                return position;

                notFound:;
            }
            return -1;
        }
    }
}