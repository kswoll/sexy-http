﻿using System.Collections.Generic;

namespace SexyHttp
{
    public static class HttpPathParser
    {
        public static HttpPath Parse(string path)
        {
            var parts = new List<HttpPathPart>();

            var lastIndex = 0;
            do
            {
                var openingBraceIndex = path.IndexOf('{', lastIndex);
                if (openingBraceIndex != -1)
                {
                    var literal = path.Substring(lastIndex, openingBraceIndex - lastIndex);
                    parts.Add(literal);

                    var closingBraceIndex = path.IndexOf('}', openingBraceIndex + 1);
                    var startIndex = openingBraceIndex + 1;
                    var key = path.Substring(startIndex, closingBraceIndex - startIndex);
                    parts.Add(new VariableHttpPathPart(key));
                    lastIndex = closingBraceIndex + 1;
                }
                else
                {
                    var literal = path.Substring(lastIndex);
                    parts.Add(literal);
                    lastIndex = path.Length;
                }
            }
            while (lastIndex < path.Length);

            return new HttpPath(parts);
        }
    }
}