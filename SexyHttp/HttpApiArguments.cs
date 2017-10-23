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
            set => storage[name] = value;
        }

        public void Remove(string name)
        {
            storage.Remove(name);
        }

        public bool TryGetValue(string name, out object value)
        {
            return storage.TryGetValue(name, out value);
        }
    }
}
