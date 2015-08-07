using System;

namespace SexyHttp.TypeConverters
{
    public static class TypeConverterExtensions
    {
        public static T ConvertTo<T>(this ITypeConverter typeConverter, object obj)
        {
            T result;
            if (!typeConverter.TryConvertTo(obj, out result))
                throw new Exception($"Could not convert '{obj}' (type {obj?.GetType().FullName}) to type {typeof(T).FullName}");
            return result;
        }
    }
}
