namespace SexyHttp
{
    public interface IHttpArgumentHandler
    {
        void ApplyArgument(HttpApiRequest request, string name, object argument);
    }
}