namespace SexyHttp
{
    public static class Http
    {
        public static T Create<T>()
        {
            return default(T);
//            var api = Proxy.CreateProxy<T>(InvocationHandler)
        }
    }
}