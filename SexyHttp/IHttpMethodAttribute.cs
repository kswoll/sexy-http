using System.Net.Http;

namespace SexyHttp
{
    public interface IHttpMethodAttribute
    {
        HttpMethod Method { get; }
        string Path { get; } 
    }
}