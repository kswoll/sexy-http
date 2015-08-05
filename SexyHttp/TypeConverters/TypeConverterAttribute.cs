using System;

namespace SexyHttp.TypeConverters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class TypeConverterAttribute : Attribute
    {
        public Type ConverterType { get; }

        public TypeConverterAttribute(Type converterType)
        {
            ConverterType = converterType;
        }
    }
}
