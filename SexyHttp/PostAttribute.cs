using System;
using System.Net.Http;

namespace SexyHttp
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PostAttribute : Attribute, IHttpMethodAttribute
    {
        public string Path { get; }
        public HttpMethod Method => HttpMethod.Post;

        public PostAttribute(string path)
        {
            Path = path;
        }
    }
}