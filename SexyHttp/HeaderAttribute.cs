using System;
using System.Linq;
using System.Reflection;

namespace SexyHttp
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = true)]
    public class HeaderAttribute : Attribute
    {
        public string Name { get; }
        public string[] Values { get; }

        public HeaderAttribute()
        {
        }

        public HeaderAttribute(string name)
        {
            Name = name;
        }

        public HeaderAttribute(string name, params string[] values)
        {
            Name = name;
            Values = values;
        }

        public static HttpHeader[] GetHeaders(ICustomAttributeProvider provider)
        {
            return provider
                .GetCustomAttributes(typeof(HeaderAttribute), true)
                .Cast<HeaderAttribute>()
                .Select(x => new HttpHeader(x.Name, x.Values))
                .ToArray();
        }
    }
}