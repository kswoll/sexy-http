using System;

namespace SexyHttp.TypeConverters
{
    /// <summary>
    /// If you have T and want T[], this converter converts to that for you
    /// </summary>
    public class ArrayTypeConverter : ITypeConverter
    {
        public bool TryConvertTo<T>(object obj, out T result)
        {
            if (typeof(T).IsArray && typeof(T).GetElementType().IsInstanceOfType(obj))
            {
                var array = Array.CreateInstance(typeof(T).GetElementType(), 1);
                array.SetValue(obj, 0);
                result = (T)(object)array;
                return true;
            }
            result = default(T);
            return false;
        }
    }
}
