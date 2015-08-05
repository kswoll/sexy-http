namespace SexyHttp
{
    public interface IHttpApiRequestInstrumenter
    {
        void InstrumentRequest(HttpApiRequest request);
    }
}