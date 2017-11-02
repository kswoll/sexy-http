using System;
using System.Text;

namespace SexyHttp.HttpBodies
{
    public class StringHttpBodyVisitor : IHttpBodyVisitor<string>
    {
        public static string GetString(HttpBody body)
        {
            return body?.Accept(new StringHttpBodyVisitor());
        }

        public string VisitJsonBody(JsonHttpBody body)
        {
            return body.ToString();
        }

        public string VisitStringBody(StringHttpBody body)
        {
            return body.Text;
        }

        public string VisitMultipartBody(MultipartHttpBody body)
        {
            throw new NotImplementedException();
        }

        public string VisitByteArrayBody(ByteArrayHttpBody body)
        {
            return Encoding.UTF8.GetString(body.Data);
        }

        public string VisitStreamBody(StreamHttpBody body)
        {
            throw new NotImplementedException();
        }

        public string VisitFormBody(FormHttpBody body)
        {
            throw new NotImplementedException();
        }
    }
}
