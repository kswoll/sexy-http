using System;

namespace SexyHttp
{
    public class NonSuccessfulResponseException : Exception
    {
        public HttpApiResponse Response { get; }

        public NonSuccessfulResponseException(HttpApiResponse response) : base($"Server responded with status: {response.StatusCode}")
        {
            Response = response;
        }
    }
}
