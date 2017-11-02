using System.Collections.Generic;
using System.Linq;

namespace SexyHttp.Urls
{
    public static class HttpUrlParser
    {
        public static HttpUrlDescriptor Parse(string url)
        {
            if (url.StartsWith("/"))
                url = url.Substring(1);

            var queryIndex = url.IndexOf('?');
            var pathSection = url;
            if (queryIndex != -1)
            {
                pathSection = url.Substring(0, queryIndex);
            }
            var pathParts = ParseChunk(pathSection).ToList();
            var queryParts = new List<KeyValuePair<string, HttpUrlPart>>();

            if (queryIndex != -1)
            {
                var querySection = url.Substring(queryIndex + 1);
                foreach (var queryPairString in querySection.Split('&'))
                {
                    var queryPair = queryPairString.Split('=');
                    if (queryPair.Length < 2)
                    {
                        throw new HttpUrlParseException($"Unable to parse '{url}': Invalid query argument '{queryPairString}'. Format should be name={{argument}}");
                    }
                    var key = queryPair[0];
                    var value = queryPair[1];

                    var queryUrlParts = ParseChunk(value).ToList();
                    if (queryUrlParts.Count != 1)
                        throw new HttpUrlParseException($"Query string values must be simple.  Either name=value (for a literal) or name={{argument}} (for an argument reference).  Invalid key is \"{key}\" in url \"{url}\"");

                    var queryPart = new KeyValuePair<string, HttpUrlPart>(key, queryUrlParts.Single());
                    queryParts.Add(queryPart);
                }
            }

            return new HttpUrlDescriptor(pathParts, queryParts);
        }

        private static IEnumerable<HttpUrlPart> ParseChunk(string chunk)
        {
            var lastIndex = 0;
            do
            {
                var openingBraceIndex = chunk.IndexOf('{', lastIndex);
                if (openingBraceIndex != -1)
                {
                    var literal = chunk.Substring(lastIndex, openingBraceIndex - lastIndex);
                    if (literal.Length > 0)
                        yield return literal;

                    var closingBraceIndex = chunk.IndexOf('}', openingBraceIndex + 1);
                    var startIndex = openingBraceIndex + 1;
                    var key = chunk.Substring(startIndex, closingBraceIndex - startIndex);
                    yield return new VariableHttpPathPart(key);
                    lastIndex = closingBraceIndex + 1;
                }
                else
                {
                    var literal = chunk.Substring(lastIndex);
                    if (literal.Length > 0)
                        yield return literal;
                    lastIndex = chunk.Length;
                }
            }
            while (lastIndex < chunk.Length);
        }
    }
}