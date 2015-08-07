using System;
using System.Collections.Concurrent;
using SexyHttp.ResponseHandlers;
using SexyHttp.TypeConverters;

namespace SexyHttp.ArgumentHandlers
{
    public class HttpResponseTypeConverter : ITypeConverter
    {
        private readonly ConcurrentDictionary<Type, Type> responseTypesByReturnType = new ConcurrentDictionary<Type, Type>();

        public HttpResponseTypeConverter()
        {
            responseTypesByReturnType[typeof(void)] = typeof(NullResponseHandler);
            responseTypesByReturnType[typeof(byte[])] = typeof(ByteArrayResponseHandler);
        }

        public bool TryConvertTo<T>(object obj, out T result)
        {
            if (typeof(T) != typeof(IHttpResponseHandler))
            {
                result = default(T);
                return false;
            }

            var type = (Type)obj;
            Type typeResult;
            if (!responseTypesByReturnType.TryGetValue(type, out typeResult))
            {
                typeResult = typeof(JsonResponseHandler);
            }
            var handler = (IHttpResponseHandler)Activator.CreateInstance(typeResult);
            result = (T)handler;
            handler.ResponseType = type;
            return true;
        }
    }
}
