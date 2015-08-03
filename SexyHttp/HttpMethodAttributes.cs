using System;
using System.Linq;
using System.Reflection;

namespace SexyHttp
{
    public static class HttpMethodAttributes
    {
        private static readonly Type[] methodAttributeTypes =
        {
            typeof(GetAttribute),
            typeof(PutAttribute),
            typeof(PostAttribute),
            typeof(DeleteAttribute)
        };

        public static IHttpMethodAttribute GetHttpMethodAttribute(this MethodInfo method)
        {
            var attributes = methodAttributeTypes.SelectMany(x => method.GetCustomAttributes(x, true)).Cast<IHttpMethodAttribute>().ToArray();
            if (attributes.Length > 1)
                throw new HttpMethodException(method, attributes);
            return attributes.SingleOrDefault();
        }
    }
}