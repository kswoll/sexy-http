using System;
using System.Reflection;
using System.Threading.Tasks;

namespace SexyHttp
{
    /// <summary>
    /// A callback you can provide when creating your API client.  It allows you to modify the request
    /// before being sent to the server and the response before being returned to the caller.
    /// </summary>
    /// <param name="endpoint">The endpoint representing the method on your interface. Contains the MethodInfo
    /// used to generate the endpoint definition. Useful if you want to interrogate its attributes, etc.</param>
    /// <param name="request">The request taht will be sent to the server</param>
    /// <param name="inner">Call this to actually make the call to the server.  It will return the
    /// response which you should return from this callback, possibly after manipulating it in
    /// some way.</param>
    /// <returns>The response to return to the caller.</returns>
    public delegate Task<HttpApiResponse> HttpApiInstrumenter(HttpApiEndpoint endpoint, HttpApiRequest request, Func<HttpApiRequest, Task<HttpApiResponse>> inner);
}