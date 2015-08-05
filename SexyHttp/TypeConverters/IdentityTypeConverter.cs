namespace SexyHttp.TypeConverters
{
    public class IdentityTypeConverter : ITypeConverter
    {
        public bool TryConvertTo<T>(object obj, out T result)
        {
            if (obj is T)
            {
                result = (T)obj;
                return true;
            }
            result = default(T);
            return false;
        }
    }
}
