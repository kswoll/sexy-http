using System;
using System.Net.Http;

namespace SexyHttp
{
    /// <summary>
    /// Decorate your API method with this attribute to indicate that the HTTP Method should be
    /// GET.
    /// </summary>
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