using System;

namespace SexyHttp.Urls
{
    public class HttpUrlFormatException : Exception
    {
        public HttpUrlFormatException(string url, string message) : base($"Error parsing url '{url}': {message}")
        {
        }
    }
}
