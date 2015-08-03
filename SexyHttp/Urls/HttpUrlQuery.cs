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
                string[] result;
                variables.TryGetValue(key, out result);
                return result;
            }
            set { variables[key] = value; }
        }
    }
}
