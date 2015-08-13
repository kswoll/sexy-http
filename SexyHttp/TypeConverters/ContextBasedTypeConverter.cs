using System;

namespace SexyHttp.TypeConverters
{
    public class ContextBasedTypeConverter : ITypeConverter
    {
        public TypeConversionContext Context { get; }
        public ITypeConverter Converter { get; }

        public ContextBasedTypeConverter(TypeConversionContext context, ITypeConverter converter)
        {
            Context = context;
            Converter = converter;
        }

        public bool TryConvertTo(ITypeConverter root, TypeConversionContext context, Type convertTo, object value, out object result)
        {
            if ((Context & context) == context)
            {
                return Converter.TryConvertTo(root, context, convertTo, value, out result);
            }
            result = null;
            return false;
        }
    }
}
