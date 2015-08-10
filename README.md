# Sexy Http
A C# HTTP client framework for interacting with HTTP-based apis (such as rest APIs) in a declarative fashion by stubbing out the
method signatures of what the API should be and generating implementations that communicate with the target server in an 
extensible way.

The basic approach is very much inspired by [refit](https://github.com/paulcbetts/refit) but provides a greater array of
extensibility points to customize the behavior of the client.

To start, the general approach is that you define a contract of C# methods that specify the nature of the interaction with the 
backend endpoint.  For example, to define a POST endpoint that takes a `string` and returns a `string`, create an interface like so:

    public interface ISampleApi
    {
        [Post]
        Task<string> PostString(int value);
    }

Before deconstructing this and explaining how the data will be serialized and deserialized, let's first see how'd you'd 
instantiate your client:

    ISampleApi client = HttpApiClient<ISampleApi>.Create("http://someserver.com", new HttpClientHandler());

And to make the call:

    string result = await client.PostString("foo");

With the API defined as above, this will make an HTTP POST request to `http://someserver.com`.  By default, arguments are 
serialized into JSON.  In this case, the argument is a string and is serialized as a JSON string, i.e. `"foo"` (vs `foo`).
This serialized text makes up the HTTP body of the request.  Important to note, 