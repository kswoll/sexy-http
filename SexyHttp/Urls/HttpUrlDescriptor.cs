﻿using System.Collections.Generic;
using System.Linq;

namespace SexyHttp.Urls
{
    public sealed class HttpUrlDescriptor
    {
        public IReadOnlyList<HttpUrlPart> PathParts { get; }
        public IReadOnlyList<KeyValuePair<string, HttpUrlPart>> QueryParts { get; }

        public HttpUrlDescriptor(IEnumerable<HttpUrlPart> pathParts, IEnumerable<KeyValuePair<string, HttpUrlPart>> queryParts)
        {
            PathParts = pathParts.ToList();
            QueryParts = queryParts.ToList();
        }

        public HttpUrlDescriptor(params HttpUrlPart[] parts) : this(parts, new KeyValuePair<string, HttpUrlPart>[0])
        {
        }

        public HttpUrlDescriptor(params KeyValuePair<string, HttpUrlPart>[] queryParts) : this(new HttpUrlPart[0], queryParts)
        {
        }

        public static implicit operator HttpUrlDescriptor(string path)
        {
            return new HttpUrlDescriptor(new LiteralHttpUrlPart(path));
        }

        public HttpUrl CreateUrl(string baseUrl)
        {
            return new HttpUrl(this, baseUrl);
        }
    }
}