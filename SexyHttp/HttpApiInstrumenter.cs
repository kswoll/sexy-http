namespace SexyHttp
{
    /// <summary>
    /// A callback you can provide when creating your API client.  It allows you to modify the request
    /// before being sent to the server and the response before being returned to the caller.
    /// </summary>
    /// <param name="endpoint">The endpoint representing the method on your interface. Contains the MethodInfo
    /// used to generate the endpoint definition. Useful if you want to interrogate its attributes, etc.</param>
    /// <param name="arguments">The arguments passed into the method. You can use this to both obtain the values
    /// passed in by the user in addition to mutating them before generating the HttpApiRequest.</param>
    /// <param name="inner">The inner instrumentation.  You should delegate calls to this object to get the
    /// request, response, and result, respectively, upon which you can add your modifications.</param>
    /// <returns>The new instrumentation that (selectively) overrides the default instrumentation.</returns>
    public delegate IHttpApiInstrumentation HttpApiInstrumenter(
        HttpApiEndpoint endpoint,
        HttpApiArguments arguments,
        IHttpApiInstrumentation inner);
}