using System;

namespace SexyHttp.TypeConverters
{
    /// <summary>
    /// If you have T and want T[], this converter converts to that for you
    /// </summary>
    public class ElementToArrayTypeConverter : ITypeConverter
    {
        public bool TryConvertTo(ITypeConverter root, Type convertTo, object obj, out object result)
        {
            if (convertTo.IsArray && convertTo.GetElementType().IsInstanceOfType(obj))
            {
                var array = Array.CreateInstance(convertTo.GetElementType(), 1);
                array.SetValue(obj, 0);
                result = array;
                return true;
            }
            result = null;
            return false;
        }
    }
}
