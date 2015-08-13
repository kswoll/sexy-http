using System;
using System.Collections.Generic;

namespace SexyHttp.TypeConverters
{
    public static class LambdaTypeConverter
    {
        public static LambdaTypeConverter<T> Create<T>(Func<ITypeConverter, object, Type, TypeConverterResult<T>> converter) 
        {
            return new LambdaTypeConverter<T>(converter);
        }

        public static LambdaTypeConverter<T> Create<T>(Func<object, Type, TypeConverterResult<T>> converter) 
        {
            return new LambdaTypeConverter<T>((root, value, convertTo) => converter(value, convertTo));
        }

        public static LambdaTypeConverter<T> Create<T>(Func<ITypeConverter, object, TypeConverterResult<T>> converter)
        {
            return Create((root, x, convertTo) => converter(root, x));
        }

        public static LambdaTypeConverter<T> Create<T>(Func<object, TypeConverterResult<T>> converter)
        {
            return Create((root, x, convertTo) => converter(x));
        }

        public static LambdaTypeConverter<T> Create<T>(Func<ITypeConverter, object, Type, T> converter)
        {
            return new LambdaTypeConverter<T>((root, x, convertTo) =>
            {
                var result = converter(root, x, convertTo);
                return new TypeConverterResult<T>(!EqualityComparer<T>.Default.Equals(result, default(T)), result);
            });
        }

        public static LambdaTypeConverter<T> Create<T>(Func<object, Type, T> converter)
        {
            return Create((root, value, convertTo) => converter(value, convertTo));
        }

        public static LambdaTypeConverter<T> Create<T>(Func<object, T> converter)
        {
            return Create((x, convertTo) => converter(x));
        }
    }

    public class LambdaTypeConverter<T> : ITypeConverter
    {
        private readonly Func<ITypeConverter, object, Type, TypeConverterResult<T>> converter;

        internal LambdaTypeConverter(Func<ITypeConverter, object, Type, TypeConverterResult<T>> converter)
        {
            this.converter = converter;
        }

        public bool TryConvertTo(ITypeConverter root, TypeConversionContext context, Type convertTo, object obj, out object result)
        {
            if (!convertTo.IsAssignableFrom(typeof(T)) && !typeof(T).IsAssignableFrom(convertTo))
            {
                result = null;
                return false;
            }

            var value = converter(root, obj, convertTo);
            result = value.Value;
            return value.IsConverted;
        }
    }
}
