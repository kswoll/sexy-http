using System;
using System.Collections.Generic;
using System.Text;

namespace SexyHttp.TypeConverters
{
    /// <summary>
    /// This is not a performant type converter when the conversion fails, since there's no way to do a 
    /// TryChangeType with System.Convert.  Thus this should be used as the ultimate fallback.
    /// </summary>
    public class SystemConvertTypeConverter : ITypeConverter
    {
        public bool TryConvertTo(ITypeConverter root, TypeConversionContext context, Type convertTo, object obj, out object result)
        {
            try
            {
                result = Convert.ChangeType(obj, convertTo);
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }
    }
}
