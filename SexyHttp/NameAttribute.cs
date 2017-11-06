using System;

namespace SexyHttp
{
    /// <summary>
    /// Use to override the expected query string key for a given method parameter.  For example,
    /// the method parameter might be "someKey" but the URL expects "some_key".  In this situation,
    /// you should use normal C# naming conventions (i.e. "someKey") but use this attribute and
    /// pass in "some_key" so that the method parameter and URL query string key gets mapped properly.
    /// </summary>
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
