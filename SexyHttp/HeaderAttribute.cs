using System;

namespace SexyHttp
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method)]
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
    }
}