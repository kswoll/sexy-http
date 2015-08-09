using System;

namespace SexyHttp
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class NameAttribute : Attribute
    {
        public string Value { get; }

        public NameAttribute(string value)
        {
            Value = value;
        }
    }
}
