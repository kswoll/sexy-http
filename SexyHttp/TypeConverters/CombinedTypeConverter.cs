namespace SexyHttp.TypeConverters
{
    public class CombinedTypeConverter : ITypeConverter
    {
        private readonly ITypeConverter[] typeConverters;

        public CombinedTypeConverter(params ITypeConverter[] typeConverters)
        {
            this.typeConverters = typeConverters;
        }

        public bool TryConvertTo<T>(object obj, out T result)
        {
            foreach (var typeConverter in typeConverters)
            {
                if (typeConverter.TryConvertTo(obj, out result))
                    return true;
            }
            result = default(T);
            return false;
        }
    }
}
