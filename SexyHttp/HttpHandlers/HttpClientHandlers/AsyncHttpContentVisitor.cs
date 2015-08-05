using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SexyHttp.HttpHandlers.HttpClientHandlers
{
    public abstract class AsyncHttpContentVisitor<T>
    {
        protected abstract Task<T> VisitStringContentAsync(StringContent content);
        protected abstract Task<T> VisitByteArrayContentAsync(ByteArrayContent content);
        protected abstract Task<T> VisitMultipartContentAsync(MultipartContent content);
        protected abstract Task<T> VisitStreamContentAsync(StreamContent content);
        protected abstract Task<T> VisitFormUrlEncodedContentAsync(FormUrlEncodedContent content);
        protected abstract Task<T> VisitMultipartFormDataContentAsync(MultipartFormDataContent content);

        public Task<T> Accept(HttpContent content)
        {
            if (content is StringContent)
                return VisitStringContentAsync((StringContent)content);
            if (content is FormUrlEncodedContent)
                return VisitFormUrlEncodedContentAsync((FormUrlEncodedContent)content);
            if (content is ByteArrayContent)
                return VisitByteArrayContentAsync((ByteArrayContent)content);
            if (content is MultipartFormDataContent)
                return VisitMultipartFormDataContentAsync((MultipartFormDataContent)content);
            if (content is MultipartContent)
                return VisitMultipartContentAsync((MultipartContent)content);
            if (content is StreamContent)
                return VisitStreamContentAsync((StreamContent)content);

            throw new Exception("Unknown HttpContent type: " + content.GetType().FullName);
        }
    }
}
