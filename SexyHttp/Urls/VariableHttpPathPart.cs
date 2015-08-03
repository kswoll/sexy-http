namespace SexyHttp.Urls
{
    public class VariableHttpPathPart : HttpUrlPart
    {
        public string Key { get; }

        public VariableHttpPathPart(string key)
        {
            Key = key;
        }
    }
}