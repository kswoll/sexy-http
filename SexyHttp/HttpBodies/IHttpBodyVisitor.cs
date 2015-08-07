namespace SexyHttp.HttpBodies
{
    public interface IHttpBodyVisitor<out T>
    {
        T VisitJsonBody(JsonHttpBody body);
        T VisitStringBody(StringHttpBody body);
        T VisitMultipartBody(MultipartHttpBody body);
        T VisitByteArrayBody(ByteArrayHttpBody body);
        T VisitStreamBody(StreamHttpBody body);
        T VisitFormBody(FormHttpBody body);
    }
}
