using System;
using System.Net.Http;

namespace SexyHttp
{
    /// <summary>
    /// Decorate your API method with this attribute to indicate that the HTTP Method should be
    /// PUT.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PutAttribute : Attribute, IHttpMethodAttribute
    {
        public string Path { get; }
        public HttpMethod Method => HttpMethod.Put;

        public PutAttribute(string path = "")
        {
            Path = path;
        }
    }
}