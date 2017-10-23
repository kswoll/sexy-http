using System.Collections.Generic;

namespace SexyHttp
{
    public class HttpApiArguments
    {
        private readonly Dictionary<string, object> storage;

        public HttpApiArguments(Dictionary<string, object> values)
        {
            storage = new Dictionary<string, object>(values);
        }

        public object this[string name]
        {
            get => storage[name];
            set
            {
                if (!storage.ContainsKey(name))
                    throw new KeyNotFoundException($"Key '{name}' is not a valid argument name for this API endpoint.");
                storage[name] = value;
            }
        }

        public bool TryGetValue(string name, out object value)
        {
            return storage.TryGetValue(name, out value);
        }
    }
}
