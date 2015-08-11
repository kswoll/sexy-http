using System;

namespace SexyHttp.TypeConverters
{
    public static class TypeConverterExtensions
    {
        public static object ConvertTo(this ITypeConverter typeConverter, Type convertTo, object obj)
        {
            object result;
            if (!typeConverter.TryConvertTo(typeConverter, convertTo, obj, out result))
                throw new Exception($"Could not convert '{obj}' (type {obj?.GetType().FullName}) to type {convertTo.FullName}");
            return result;
        }

        public static bool TryConvertTo<T>(this ITypeConverter typeConverter, object obj, out T result)
        {
            object value;
            var converted = typeConverter.TryConvertTo(typeConverter, typeof(T), obj, out value);
            if (converted)
            {
                result = (T)value;
                return true;
            }
            else
            {
                result = default(T);
                return false;
            }
        }

        public static T ConvertTo<T>(this ITypeConverter typeConverter, object obj)
        {
            return (T)typeConverter.ConvertTo(typeof(T), obj);
        }
    }
}
