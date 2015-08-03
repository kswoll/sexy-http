using System.Collections.Generic;

namespace SexyHttp
{
    public interface IHttpPathPart
    {
        string ToString(Dictionary<string, object> arguments);
    }
}