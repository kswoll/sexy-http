namespace SexyHttp.TypeConverters
{
    public class DefaultTypeConverter : CombinedTypeConverter
    {
        public DefaultTypeConverter() : base(CreateRegistryTypeConverter(), new IdentityTypeConverter())
        {
        }

        private static RegistryTypeConverter CreateRegistryTypeConverter()
        {
            var result = new RegistryTypeConverter();
            return result;
        }
    }
}
