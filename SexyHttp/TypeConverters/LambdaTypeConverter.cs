using System;
using System.Collections.Generic;

namespace SexyHttp.TypeConverters
{
    public static class LambdaTypeConverter
    {
        public static LambdaTypeConverter<T> Create<T>(Func<object, Type, TypeConverterResult<T>> converter) 
        {
            return new LambdaTypeConverter<T>(converter);
        }

        public static LambdaTypeConverter<T> Create<T>(Func<object, TypeConverterResult<T>> converter)
        {
            return Create((x, convertTo) => converter(x));
        }

        public static LambdaTypeConverter<T> Create<T>(Func<object, Type, T> converter) 
        {
            return new LambdaTypeConverter<T>((x, convertTo) =>
            {
                var result = converter(x, convertTo);
                return new TypeConverterResult<T>(!EqualityComparer<T>.Default.Equals(result, default(T)), result);
            });
        }

        public static LambdaTypeConverter<T> Create<T>(Func<object, T> converter)
        {
            return Create((x, convertTo) => converter(x));
        }
    }

    public class LambdaTypeConverter<T> : ITypeConverter
    {
        private readonly Func<object, Type, TypeConverterResult<T>> converter;

        internal LambdaTypeConverter(Func<object, Type, TypeConverterResult<T>> converter)
        {
            this.converter = converter;
        }

        public bool TryConvertTo(ITypeConverter root, Type convertTo, object obj, out object result)
        {
            if (!convertTo.IsAssignableFrom(typeof(T)) && !typeof(T).IsAssignableFrom(convertTo))
            {
                result = null;
                return false;
            }

            var value = converter(obj, convertTo);
            result = value.Value;
            return value.IsConverted;
        }
    }
}
