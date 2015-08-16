using System;

namespace SexyHttp
{
    public class NonSuccessfulResponseException : Exception
    {
        public HttpApiRequest Request { get; }
        public HttpApiResponse Response { get; }

        public NonSuccessfulResponseException(HttpApiRequest request, HttpApiResponse response) : base($"Server responded with status: {response.StatusCode} when contacting {request.Url}")
        {
            Request = request;
            Response = response;
        }
    }
}
