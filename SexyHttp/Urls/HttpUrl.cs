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
        public HttpUrlDescriptor Descriptor { get; set; }

        public HttpUrl(HttpUrlDescriptor descriptor, string baseUrl)
        {
            Descriptor = descriptor;
            BaseUrl = baseUrl;
            Path = new HttpUrlPath();
            Query = new HttpUrlQuery();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(BaseUrl);

            if (Descriptor.PathParts.Any())
            {
                builder.Append('/');

                foreach (var part in Descriptor.PathParts)
                {
                    var literal = part as LiteralHttpUrlPart;
                    if (literal != null)
                        builder.Append(literal.Value);
                    else
                        builder.Append(Path[((VariableHttpPathPart)part).Key]);
                }
            }

            if (Descriptor.QueryParts.Any())
            {
                var separator = '?';
                Action<string, string> appendQuery = (key, value) =>
                {
                    if (value != null)
                    {
                        builder.Append(separator);
                        builder.Append(Uri.EscapeDataString(key));
                        builder.Append('=');
                        builder.Append(Uri.EscapeDataString(value).Replace("%2C", ","));        // Hack to preserve commas
                        separator = '&';
                    }
                };
                foreach (var item in Descriptor.QueryParts)
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