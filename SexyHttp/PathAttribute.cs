using System;

namespace SexyHttp
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class PathAttribute : Attribute
    {
        public string Path { get; }

        public PathAttribute(string path)
        {
            Path = path;
        }
    }
}
