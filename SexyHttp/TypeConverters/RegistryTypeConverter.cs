using System;
using System.Collections.Concurrent;

namespace SexyHttp.TypeConverters
{
    public class RegistryTypeConverter : ITypeConverter
    {
        private readonly ConcurrentDictionary<Tuple<Type, Type>, ITypeConverter> typeConverters = new ConcurrentDictionary<Tuple<Type, Type>, ITypeConverter>();

        public void Register(Type sourceType, Type targetType, ITypeConverter typeConverter)
        {
            typeConverters[Tuple.Create(sourceType, targetType)] = typeConverter;
        }

        public void Register<TSourceType, TTargetType>(ITypeConverter typeConverter)
        {
            typeConverters[Tuple.Create(typeof(TSourceType), typeof(TTargetType))] = typeConverter;
        }

        public bool TryConvertTo<T>(object obj, out T result)
        {
            var targetType = typeof(T);

            while (targetType != null)
            {
                var sourceType = obj?.GetType() ?? typeof(void);
                while (sourceType != null)
                {
                    var key = Tuple.Create(sourceType, targetType);
                    ITypeConverter converter;
                    if (typeConverters.TryGetValue(key, out converter))
                    {
                        if (converter.TryConvertTo(obj, out result))
                        {
                            return true;
                        }
                    }
                    sourceType = sourceType.BaseType;
                }
                targetType = targetType.BaseType;
            }

            result = default(T);
            return false;
        }
    }
}
