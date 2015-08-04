namespace SexyHttp.TypeConverters
{
    public static class TypeConverterExtensions
    {
        public static T ConvertTo<T>(this ITypeConverter typeConverter, object obj)
        {
            T result;
            typeConverter.TryConvertTo(obj, out result);
            return result;
        }
    }
}
