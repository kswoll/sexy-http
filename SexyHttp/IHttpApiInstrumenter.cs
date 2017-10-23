namespace SexyHttp
{
    public interface IHttpApiInstrumenter
    {
        IHttpApiInstrumentation InstrumentCall(
            HttpApiEndpoint endpoint,
            HttpApiArguments arguments,
            IHttpApiInstrumentation inner);
    }
}