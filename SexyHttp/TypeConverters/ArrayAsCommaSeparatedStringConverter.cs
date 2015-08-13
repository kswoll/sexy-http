using System;
using System.Linq;

namespace SexyHttp.TypeConverters
{
    public class ArrayAsCommaSeparatedStringConverter : ITypeConverter
    {
        public bool TryConvertTo(ITypeConverter root, TypeConversionContext context, Type convertTo, object value, out object result)
        {
            if (convertTo == typeof(string[]) && (value?.GetType().IsArray ?? false))
            {
                result = new[] { string.Join(",", ((Array)value).Cast<object>().Select(x => root.ConvertTo<string>(context, x))) };
                return true;
            }
            result = null;
            return false;
        }
    }
}