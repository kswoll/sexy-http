using System;

namespace SexyHttp.TypeConverters
{
    /// <summary>
    /// If you have T and want T[], this converter converts to that for you
    /// </summary>
    public class ElementToArrayTypeConverter : ITypeConverter
    {
        public bool TryConvertTo(ITypeConverter root, TypeConversionContext context, Type convertTo, object value, out object result)
        {
            if (convertTo.IsArray)
            {
                var array = Array.CreateInstance(convertTo.GetElementType(), 1);
                var typedValue = root.ConvertTo(context, convertTo.GetElementType(), value);
                array.SetValue(typedValue, 0);
                result = array;
                return true;
            }
            result = null;
            return false;
        }
    }
}
