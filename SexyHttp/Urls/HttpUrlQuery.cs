using System.Collections.Generic;

namespace SexyHttp.Urls
{
    public class HttpUrlQuery
    {
        private readonly Dictionary<string, string[]> variables = new Dictionary<string, string[]>();

        public string[] this[string key]
        {
            get
            {
                variables.TryGetValue(key, out var result);
                return result;
            }
            set => variables[key] = value;
        }

        public void CopyFrom(HttpUrlQuery source)
        {
            foreach (var item in source.variables)
            {
                variables[item.Key] = item.Value;
            }
        }
    }
}
