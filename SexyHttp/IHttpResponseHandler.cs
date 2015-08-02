namespace SexyHttp
{
    public interface IHttpResponseHandler
    {
        object HandleResponse(HttpApiResponse response);
    }
}