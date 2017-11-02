using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace SexyHttp.Urls
{
    internal static class EnumMemberCache
    {
        private static readonly ConcurrentDictionary<Type, Dictionary<Enum, string>> enumNamesByType = new ConcurrentDictionary<Type, Dictionary<Enum, string>>();
        private static readonly ConcurrentDictionary<Type, Dictionary<string, Enum>> enumsByType = new ConcurrentDictionary<Type, Dictionary<string, Enum>>();

        private static IReadOnlyDictionary<string, Enum> GetCache(Type type)
        {
            Dictionary<string, Enum> cache = enumsByType.GetOrAdd(type, _ => type
                .GetFields()
                .Where(x => x.IsStatic && x.IsPublic)
                .Select(x => new
                {
                    x.Name,
                    Value = x.GetValue(null),
                    Attribute = (EnumMemberAttribute)x.GetCustomAttribute(typeof(EnumMemberAttribute))
                })
                .ToDictionary(x => x.Attribute != null ? x.Attribute.Value : x.Name, x => (Enum)x.Value));
            return cache;
        }

        public static IEnumerable<Enum> GetEnumValues(Type type)
        {
            return GetCache(type).Values;
        }

        public static Enum GetEnumMemberByName(Type type, string name, Enum defaultEnum = null)
        {
            return GetCache(type).Get(name, () => defaultEnum);
        }

        public static T GetEnumMemberByNameOrDefault<T>(string name, T defaultEnum = default(T)) where T : struct
        {
            if (TryGetEnumMemberByName(name, out T enumMember, defaultEnum))
            {
                return enumMember;
            }

            return defaultEnum;
        }

        public static bool TryGetEnumMemberByName<T>(string name, out T enumMember, T defaultEnum = default(T)) where T : struct
        {
            if (GetCache(typeof(T)).TryGetValue(name, out var result))
            {
                enumMember = (T)(object)result;
                return true;
            }
            else
            {
                enumMember = defaultEnum;
                return false;
            }
        }

        public static string GetEnumMemberName(Enum value)
        {
            Dictionary<Enum, string> cache = enumNamesByType.GetOrAdd(value.GetType(), _ => value.GetType()
                .GetFields()
                .Where(x => x.IsStatic && x.IsPublic)
                .Select(x => new
                {
                    x.Name,
                    Value = x.GetValue(null),
                    Attribute = (EnumMemberAttribute)x.GetCustomAttribute(typeof(EnumMemberAttribute))
                })
                .ToDictionary(x => (Enum)x.Value, x => x.Attribute != null ? x.Attribute.Value : x.Name));
            return cache.Get(value);
        }

        private static TValue Get<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> ifNotFound = null)
        {
            if (dictionary == null || (!dictionary.TryGetValue(key, out TValue result) && ifNotFound != null))
            {
                result = ifNotFound != null ? ifNotFound() : default(TValue);
            }
            return result;
        }
    }
}
