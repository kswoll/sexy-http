using System;
using System.Linq;
using System.Reflection;

namespace SexyHttp.TypeConverters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true)]
    public class TypeConverterAttribute : Attribute
    {
        public Type ConverterType { get; }
        public TypeConversionContext Context { get; }

        public TypeConverterAttribute(Type converterType, TypeConversionContext context = TypeConversionContext.All)
        {
            ConverterType = converterType;
            Context = context;
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
            return attributes
                .Select(x => new { x.Context, Converter = (ITypeConverter)Activator.CreateInstance(x.ConverterType) })
                .Select(x => x.Context == TypeConversionContext.None ? x.Converter : new ContextBasedTypeConverter(x.Context, x.Converter))
                .ToArray();
        }
    }
}
