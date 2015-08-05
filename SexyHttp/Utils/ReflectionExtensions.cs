using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SexyHttp.Utils
{
    internal static class ReflectionExtensions
    {
        public static Type GetTaskType(this Type type)
        {
            var current = type;
            while (current != null)
            {
                if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(Task<>))
                    return current.GetGenericArguments()[0];
                current = current.BaseType;
            }
            return null;
        }
    }
}
