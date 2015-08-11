using System;

namespace SexyHttp.TypeConverters
{
    public interface ITypeConverter
    {
        bool TryConvertTo(Type convertTo, object obj, out object result);
    }
}
