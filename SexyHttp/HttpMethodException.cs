using System;
using System.Linq;
using System.Reflection;

namespace SexyHttp
{
    public class HttpMethodException : Exception
    {
        public HttpMethodException(MethodInfo method, IHttpMethodAttribute[] attributes) : 
            base($"Method {method.DeclaringType.FullName}.{method.Name} defines multiple HTTP method attributes ({string.Join(", ", attributes.Select(x => x.GetType().Name))}), only one is allowed.")
        {
        }
    }
}