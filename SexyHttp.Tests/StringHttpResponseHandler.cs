namespace SexyHttp
{
    public class MockHttpResponseHandler : IHttpResponseHandler
    {
        public object Value { get; }

        public MockHttpResponseHandler(object value)
        {
            Value = value;
        }

        public object HandleResponse(HttpApiResponse response)
        {
            return Value;
        }
    }
}