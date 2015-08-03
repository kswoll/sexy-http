using System;
using System.Net.Http;

namespace SexyHttp
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PutAttribute : Attribute, IHttpMethodAttribute
    {
        public string Path { get; }
        public HttpMethod Method => HttpMethod.Put;

        public PutAttribute(string path)
        {
            Path = path;
        }
    }
}