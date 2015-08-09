using System;
using System.Collections.Generic;
using System.Linq;

namespace SexyHttp.Utils
{
    public static class DictionaryExtensions
    {
        internal static Dictionary<string, T> ObjectToDictionary<T>(this object obj, Func<object, T> valueConverter)
        {
            var result = new Dictionary<string, T>();
            foreach (var property in obj.GetType().GetProperties().Where(x => x.CanRead))
            {
                var value = property.GetValue(obj, null);
                result[property.Name] = valueConverter(value);
            }
            return result;
        }

        internal static Dictionary<string, object> ObjectToDictionary(this object obj)
        {
            return obj.ObjectToDictionary(x => x);
        }
    }
}
