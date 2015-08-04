namespace SexyHttp.TypeConverters
{
    public interface ITypeConverter
    {
        bool TryConvertTo<T>(object obj, out T result);
    }
}
