using System;

namespace SexyHttp.TypeConverters
{
    public class IdentityTypeConverter : ITypeConverter
    {
        public bool TryConvertTo(Type convertTo, object obj, out object result)
        {
            if (convertTo.IsInstanceOfType(obj))
            {
                result = obj;
                return true;
            }
            result = null;
            return false;
        }
    }
}
