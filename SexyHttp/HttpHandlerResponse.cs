using System;

namespace SexyHttp
{
    public struct HttpHandlerResponse
    {
        public HttpApiResponse ApiResponse { get; }
        public TimeSpan RequestWriteTime { get; }
        public TimeSpan ResponseReadTime { get; }

        public HttpHandlerResponse(HttpApiResponse apiResponse, TimeSpan requestWriteTime, TimeSpan responseReadTime)
        {
            ApiResponse = apiResponse;
            RequestWriteTime = requestWriteTime;
            ResponseReadTime = responseReadTime;
        }
    }
}
