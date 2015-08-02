using System;

namespace SexyHttp
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GetAttribute : Attribute
    {
        public string Path { get; }

        public GetAttribute(string path)
        {
            Path = path;
        }
    }
}