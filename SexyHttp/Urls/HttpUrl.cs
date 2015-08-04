using System;
using System.Linq;
using System.Text;

namespace SexyHttp.Urls
{
    public class HttpUrl
    {
        public string BaseUrl { get; set; }
        public HttpUrlPath Path { get; set; }
        public HttpUrlQuery Query { get; set; }

        private readonly HttpUrlDescriptor descriptor;

        public HttpUrl(HttpUrlDescriptor descriptor, string baseUrl)
        {
            this.descriptor = descriptor;
            BaseUrl = baseUrl;
            Path = new HttpUrlPath();
            Query = new HttpUrlQuery();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(BaseUrl);

            if (descriptor.PathParts.Any())
            {
                builder.Append('/');

                foreach (var part in descriptor.PathParts)
                {
                    var literal = part as LiteralHttpUrlPart;
                    if (literal != null)
                        builder.Append(literal.Value);
                    else
                        builder.Append(Path[((VariableHttpPathPart)part).Key]);
                }                
            }

            if (descriptor.QueryParts.Any())
            {
                var separator = '?';
                Action<string, string> appendQuery = (key, value) =>
                {
                    builder.Append(separator);
                    builder.Append(Uri.EscapeDataString(key));
                    builder.Append('=');
                    builder.Append(Uri.EscapeDataString(value));
                    separator = '&';
                };
                foreach (var item in descriptor.QueryParts)
                {
                    var part = item.Value;
                    var literal = part as LiteralHttpUrlPart;
                    if (literal != null)
                    {
                        appendQuery(item.Key, literal.Value);
                    }
                    else
                    {
                        var variable = (VariableHttpPathPart)part;
                        var values = Query[variable.Key];
                        if (values != null)
                        {
                            foreach (var value in values)
                                appendQuery(item.Key, value);
                        }
                    }
                }
            }
            return builder.ToString();
        }

    }
}