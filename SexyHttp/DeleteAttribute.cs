using System;
using System.Net.Http;

namespace SexyHttp
{
    /// <summary>
    /// Decorate your API method with this attribute to indicate that the HTTP Method should be
    /// DELETE.
    /// </summary>
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