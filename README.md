# Sexy Http
A C# HTTP client framework for interacting with HTTP-based apis (such as rest APIs) in a declarative fashion by stubbing out the
method signatures of what the API should be and generating implementations that communicate with the target server in an 
extensible way.

The basic approach is very much inspired by [refit](https://github.com/paulcbetts/refit) but provides a greater array of
extensibility points to customize the behavior of the client.

To start, the general approach is that you define a contract of C# methods that specify the nature of the interaction with the 
backend endpoint.  For example, to define an endpoint that takes an `int` and returns a `string`, create an interface like so:

[Proxy]
public interface ISampleApi
{
    [Post]
}