using System;
using System.Threading.Tasks;
using SexyHttp.TypeConverters;

namespace SexyHttp
{
    public abstract class HttpResponseHandler : IHttpResponseHandler
    {
        protected abstract Task<object> ProvideResult(HttpApiResponse response);

        public bool NonSuccessThrowsException { get; protected set; }
        public virtual ITypeConverter TypeConverter { get; set; }
        public virtual Type ResponseType { get; set;  }

        protected HttpResponseHandler()
        {
            NonSuccessThrowsException = true;
        }

        public Task<object> HandleResponse(HttpApiResponse response)
        {
            if (NonSuccessThrowsException && ((int)response.StatusCode < 200 || (int)response.StatusCode >= 300))
            {
                throw new NonSuccessfulResponseException(response);
            }
            return ProvideResult(response);
        }
    }
}
