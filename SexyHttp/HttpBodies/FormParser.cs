using System;
using System.IO;
using System.Linq;

namespace SexyHttp.HttpBodies
{
    public class FormParser
    {
        public static FormHttpBody ParseForm(Stream input)
        {
            using (var reader = new StreamReader(input))
            {
                var s = reader.ReadToEnd();
                var values = s.Split('&')
                    .Select(x => x.Split('='))
                    .Select(x => new { Name = x[0], Value = x[1] })
                    .ToDictionary(x => x.Name, x => Uri.UnescapeDataString(x.Value));

                return new FormHttpBody(values);
            }
        } 
    }
}