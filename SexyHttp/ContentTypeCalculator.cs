using SexyHttp.HttpBodies;

namespace SexyHttp
{
    public class ContentTypeCalculator : IHttpBodyVisitor<string>
    {
        public string VisitJsonBody(JsonHttpBody body)
        {
            return "application/json";
        }

        public string VisitStringBody(StringHttpBody body)
        {
            return "text/plain";
        }

        public string VisitMultipartBody(MultipartHttpBody body)
        {
            return null;
        }

        public string VisitByteArrayBody(ByteArrayHttpBody body)
        {
            return "application/octet-stream";
        }

        public string VisitStreamBody(StreamHttpBody body)
        {
            return "application/octet-stream";
        }

        public string VisitFormBody(FormHttpBody body)
        {
            return "application/x-www-form-urlencoded";
        }
    }
}