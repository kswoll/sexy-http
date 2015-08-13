using System;

namespace SexyHttp.TypeConverters
{
    public class CombinedTypeConverter : ITypeConverter
    {
        private readonly ITypeConverter[] typeConverters;

        public CombinedTypeConverter(params ITypeConverter[] typeConverters)
        {
            this.typeConverters = typeConverters;
        }

        public bool TryConvertTo(ITypeConverter root, TypeConversionContext context, Type convertTo, object value, out object result)
        {
            foreach (var typeConverter in typeConverters)
            {
                if (typeConverter.TryConvertTo(root, context, convertTo, value, out result))
                    return true;
            }
            result = null;
            return false;
        }
    }
}
