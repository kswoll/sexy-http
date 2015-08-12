using System;

namespace SexyHttp.TypeConverters
{
    public class TypeConversionException : Exception
    {
        public object Value { get; }
        public Type ConvertTo { get; }

        public TypeConversionException(object value, Type convertTo) : base($"Could not convert value '{value}' (of type '{value?.GetType()?.FullName}') to type '{convertTo.FullName}'")
        {
            Value = value;
            ConvertTo = convertTo;
        }
    }
}
