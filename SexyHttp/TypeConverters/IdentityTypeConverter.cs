using System;

namespace SexyHttp.TypeConverters
{
    public class IdentityTypeConverter : ITypeConverter
    {
        public bool TryConvertTo(ITypeConverter root, TypeConversionContext context, Type convertTo, object value, out object result)
        {
            if (convertTo.IsInstanceOfType(value))
            {
                result = value;
                return true;
            }
            result = null;
            return false;
        }
    }
}
