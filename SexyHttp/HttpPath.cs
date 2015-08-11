namespace SexyHttp
{
    public static class HttpPath
    {
        public static string Combine(string path1, string path2)
        {
            if (path1.EndsWith("/"))
                return path1 + path2;
            else
                return path1 + "/" + path2;
        }
    }
}
