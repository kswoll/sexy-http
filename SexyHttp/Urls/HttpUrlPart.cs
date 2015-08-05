namespace SexyHttp.Urls
{
    public abstract class HttpUrlPart
    {
        public static implicit operator HttpUrlPart(string literal)
        {
            return new LiteralHttpUrlPart(literal);
        }
    }
}