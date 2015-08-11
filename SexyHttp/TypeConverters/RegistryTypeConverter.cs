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

        private Type GetBaseType(Type type)
        {
            if (type.IsValueType && type.BaseType == null)
            {
                return typeof(object);
            }
            else if (type.IsArray)
            {
                var elementType = type.GetElementType();
                if (elementType.IsValueType && elementType.BaseType == null)
                    return type.BaseType;
                else if (elementType.BaseType != null)
                    return elementType.BaseType.MakeArrayType();
                else
                    return type.BaseType;
            }
            else
            {
                return type.BaseType;
            }            
        }

        public bool TryConvertTo(Type convertTo, object obj, out object result)
        {
            var targetType = convertTo;

            while (targetType != null)
            {
                var sourceType = obj?.GetType() ?? typeof(void);
                while (sourceType != null)
                {
                    var key = Tuple.Create(sourceType, targetType);
                    ITypeConverter converter;
                    if (typeConverters.TryGetValue(key, out converter))
                    {
                        if (converter.TryConvertTo(convertTo, obj, out result))
                        {
                            return true;
                        }
                    }
                    sourceType = GetBaseType(sourceType);
                }
                targetType = GetBaseType(targetType);
            }

            result = null;
            return false;
        }
    }
}
