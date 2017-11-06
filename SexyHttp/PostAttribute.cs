using System;
using System.Net.Http;

namespace SexyHttp
{
    /// <summary>
    /// Decorate your API method with this attribute to indicate that the HTTP Method should be
    /// POST.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PostAttribute : Attribute, IHttpMethodAttribute
    {
        public string Path { get; }
        public HttpMethod Method => HttpMethod.Post;

        public PostAttribute(string path = "")
        {
            Path = path;
        }
    }
}