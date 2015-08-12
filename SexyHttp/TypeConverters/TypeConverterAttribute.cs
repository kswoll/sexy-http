using System;
using System.Linq;
using System.Reflection;

namespace SexyHttp.TypeConverters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true)]
    public class TypeConverterAttribute : Attribute
    {
        public Type ConverterType { get; }

        public TypeConverterAttribute(Type converterType)
        {
            ConverterType = converterType;
        }

        public static ITypeConverter Combine(ICustomAttributeProvider attributeProvider, ITypeConverter typeConverter)
        {
            var results = GetTypeConverters(attributeProvider);
            if (!results.Any())
                return typeConverter;
            else
                return new CombinedTypeConverter(results.Concat(new[] { typeConverter }).ToArray());
        }

        public static ITypeConverter[] GetTypeConverters(ICustomAttributeProvider attributeProvider)
        {
            var attributes = attributeProvider.GetCustomAttributes(typeof(TypeConverterAttribute), true).Cast<TypeConverterAttribute>();
            return attributes.Select(x => x.ConverterType).Select(x => (ITypeConverter)Activator.CreateInstance(x)).ToArray();
        }
    }
}
