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
        public bool TryConvertTo<T>(object obj, out T result)
        {
            try
            {
                result = (T)Convert.ChangeType(obj, typeof(T));
                return true;
            }
            catch (Exception)
            {
                result = default(T);
                return false;
            }
        }
    }
}
