using System;
using System.Net.Http;

namespace SexyHttp
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DeleteAttribute : Attribute, IHttpMethodAttribute
    {
        public string Path { get; }
        public HttpMethod Method => HttpMethod.Delete;

        public DeleteAttribute(string path = "")
        {
            Path = path;
        }
    }
}