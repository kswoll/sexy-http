using System;
using System.Net.Http;

namespace SexyHttp
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GetAttribute : Attribute, IHttpMethodAttribute
    {
        public string Path { get; }
        public HttpMethod Method => HttpMethod.Get;

        public GetAttribute(string path = "")
        {
            Path = path;
        }
    }
}