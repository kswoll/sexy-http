namespace SexyHttp
{
    public class HttpApiClient<T>
    {
        private readonly HttpApi<T> api;
        private readonly string baseUrl;

        public HttpApiClient(HttpApi<T> api, string baseUrl)
        {
            this.api = api;
            this.baseUrl = baseUrl;
        }
    }
}