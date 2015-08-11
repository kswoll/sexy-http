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

        public bool TryConvertTo(Type convertTo, object obj, out object result)
        {
            foreach (var typeConverter in typeConverters)
            {
                if (typeConverter.TryConvertTo(convertTo, obj, out result))
                    return true;
            }
            result = null;
            return false;
        }
    }
}
