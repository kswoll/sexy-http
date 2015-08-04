using System;
using System.Collections.Generic;

namespace SexyHttp.TypeConverters
{
    public static class LambdaTypeConverter
    {
        public static LambdaTypeConverter<T> Create<T>(Func<object, LambdaTypeConverter<T>.Result> converter) 
        {
            return new LambdaTypeConverter<T>(converter);
        }

        public static LambdaTypeConverter<T> Create<T>(Func<object, T> converter) 
        {
            return new LambdaTypeConverter<T>(x =>
            {
                var result = converter(x);
                return new LambdaTypeConverter<T>.Result(!EqualityComparer<T>.Default.Equals(result, default(T)), result);
            });
        }
    }

    public class LambdaTypeConverter<T> : ITypeConverter
    {
        private readonly Func<object, Result> converter;

        internal LambdaTypeConverter(Func<object, Result> converter)
        {
            this.converter = converter;
        }

        public bool TryConvertTo<TResult>(object obj, out TResult result)
        {
            if (typeof(T) != typeof(TResult))
            {
                result = default(TResult);
                return false;
            }

            var value = converter(obj);
            result = (TResult)(object)value.Value;
            return value.IsConverted;
        }

        public struct Result
        {
            public bool IsConverted { get; }
            public T Value { get; }

            public Result(bool isConverted, T value)
            {
                IsConverted = isConverted;
                Value = value;
            }
        }
    }
}
