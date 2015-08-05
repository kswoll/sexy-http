using System;
using System.Linq;
using System.Reflection;

namespace SexyHttp.TypeConverters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class TypeConverterAttribute : Attribute
    {
        public Type ConverterType { get; }

        public TypeConverterAttribute(Type converterType)
        {
            ConverterType = converterType;
        }

        public static ITypeConverter Combine(ICustomAttributeProvider attributeProvider, ITypeConverter typeConverter)
        {
            var result = GetTypeConverter(attributeProvider);
            if (result == null)
                return typeConverter;
            else
                return new CombinedTypeConverter(result, typeConverter);
        }

        public static ITypeConverter GetTypeConverter(ICustomAttributeProvider attributeProvider)
        {
            var attribute = (TypeConverterAttribute)attributeProvider.GetCustomAttributes(typeof(TypeConverterAttribute), true).SingleOrDefault();
            if (attribute != null)
                return (ITypeConverter)Activator.CreateInstance(attribute.ConverterType);
            else
                return null;
        }
    }
}
