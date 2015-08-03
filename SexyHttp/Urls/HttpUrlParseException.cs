using System;

namespace SexyHttp.Urls
{
    public class HttpUrlParseException : Exception
    {
        public HttpUrlParseException(string message) : base(message)
        {
        }
    }
}
