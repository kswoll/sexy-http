using System;

namespace SexyHttp.TypeConverters
{
    public interface ITypeConverter
    {
        bool TryConvertTo(ITypeConverter root, Type convertTo, object obj, out object result);
    }
}
