using Newtonsoft.Json.Linq;

namespace SexyHttp
{
    public class JsonHttpBody : HttpBody
    {
        public JToken Json { get; }

        public JsonHttpBody(JToken json)
        {
            Json = json;
        }
    }
}
