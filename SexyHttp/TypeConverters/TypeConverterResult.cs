using System;
using System.Collections.Generic;
using System.Text;

namespace SexyHttp.TypeConverters
{
    public static class TypeConverterResult
    {
        public static TypeConverterResult<T> Success<T>(T value)
        {
            return new TypeConverterResult<T>(true, value);
        }

        public static TypeConverterResult<T> Failure<T>()
        {
            return new TypeConverterResult<T>(false, default(T));
        }
    }

    public struct TypeConverterResult<T>
    {
        public bool IsConverted { get; }
        public T Value { get; }

        public TypeConverterResult(bool isConverted, T value)
        {
            IsConverted = isConverted;
            Value = value;
        }
    }
}
