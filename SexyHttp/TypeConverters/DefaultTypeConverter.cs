namespace SexyHttp.TypeConverters
{
    public class DefaultTypeConverter : CombinedTypeConverter
    {
        public DefaultTypeConverter() : base(CreateRegistryTypeConverter(), new IdentityTypeConverter(), new ArrayTypeConverter())
        {
        }

        private static RegistryTypeConverter CreateRegistryTypeConverter()
        {
            var result = new RegistryTypeConverter();
            return result;
        }
    }
}
