namespace SexyHttp.TypeConverters
{
    public class CombinedTypeConverter : ITypeConverter
    {
        private readonly ITypeConverter first;
        private readonly ITypeConverter second;

        public CombinedTypeConverter(ITypeConverter first, ITypeConverter second)
        {
            this.first = first;
            this.second = second;
        }

        public bool TryConvertTo<T>(object obj, out T result)
        {
            if (first.TryConvertTo(obj, out result))
                return true;
            if (second.TryConvertTo(obj, out result))
                return true;
            return false;
        }
    }
}
