using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SexyHttp.HttpBodies;
using SexyHttp.Tests.Utils;

namespace SexyHttp.ResponseHandlers
{
    public class ByteArrayResponseHandler : HttpResponseHandler
    {
        protected async override Task<object> ProvideResult(HttpApiResponse response)
        {
            var body = response.Body;
            var byteArray = await body.AcceptAsync(new ByteArrayExtractor());
            return byteArray;
        }

        private class ByteArrayExtractor : IAsyncHttpBodyVisitor<byte[]>
        {
            public Task<byte[]> VisitJsonBodyAsync(JsonHttpBody body)
            {
                return Task.FromResult(Encoding.UTF8.GetBytes(body.Json.ToString(Formatting.None)));
            }

            public Task<byte[]> VisitStringBodyAsync(StringHttpBody body)
            {
                return Task.FromResult(Encoding.UTF8.GetBytes(body.Text));
            }

            public Task<byte[]> VisitMultipartBodyAsync(MultipartHttpBody body)
            {
                throw new NotImplementedException("Multipart response not implemented");
            }

            public Task<byte[]> VisitByteArrayBodyAsync(ByteArrayHttpBody body)
            {
                return Task.FromResult(body.Data);
            }

            public Task<byte[]> VisitStreamBodyAsync(StreamHttpBody body)
            {
                return body.Stream.ReadToEndAsync();
            }

            public Task<byte[]> VisitFormBodyAsync(FormHttpBody body)
            {
                throw new NotImplementedException("Form response not supported");
            }
        }
    }
}
