# Sexy Http
A C# HTTP client framework for interacting with HTTP-based apis (such as rest APIs) in a declarative fashion by stubbing out the
method signatures of what the API should be and generating implementations that communicate with the target server in an 
extensible way.

## Overview

The basic design is very much inspired by [refit](https://github.com/paulcbetts/refit) but provides a greater array of
extensibility points to customize the behavior of the client.  Like refit, it is also async-only.  Unlike refit, you can 
define your contracts via interfaces or abstract classes.  The advantage of the latter is that it allows you to easily add 
your own arbitrary methods onto your API's interface.  Furthermore, when using the Fody version of the proxies, you can even
implement an API method -- tweak the parameters or result as you see fit, while still being able to trivially invoke the default
behavior of making the HTTP request.

One of the key extensibility points is the ability to provide custom type converters at several different layers.  A type
converter is simply an implementation of the provided `ITypeConverter` interface which looks like this:

    public interface ITypeConverter
    {
        bool TryConvertTo(ITypeConverter root, TypeConversionContext context, Type convertTo, 
            object value, out object result);
    }

A custom type converter can be provided for arguments, return values, endpoint methods, or the api type itself.  

## Installation

To add this library to your project, install using Nuget:

> Install-Package SexyHttp

## Getting Started

To start, the general approach is that you define a contract of C# methods that specify the nature of the interaction with the 
backend endpoint.  For example, to define a POST endpoint that takes a `string` and returns a `string`, create an interface like so:

    public interface ISampleApi
    {
        [Post]
        Task<string> PostString(string value);
    }

Before deconstructing this and explaining how the data will be serialized and deserialized, let's first see how'd you'd 
instantiate your client:

    ISampleApi client = HttpApiClient<ISampleApi>.Create("http://someserver.com", new HttpClientHandler());

And to make the call:

    string result = await client.PostString("foo");

With the API defined as above, this will make an HTTP POST request to `http://someserver.com`.  By default, when there is 
only one body argument (vs path, query string, etc. arguments -- details below) that argument is serialized directly as the 
JSON body.  Thus the body of your HTTP request will be:

    "foo"

### Serializing as a JSON object

But what if you wanted the body to just be a JSON object with one value where the key is the parameter name and the value is
the argument.  To do this, decorate your parameter with `[Object]`.  

    public interface ISampleApi
    {
        [Post]
        Task<string> PostString([Object]string value);
    }

With this attribute in place, the HTTP body of your request would be:

    { "value": "foo" }

Furthermore, this works with multiple parameters:

    public interface ISampleApi
    {
        [Post]
        Task<string> PostString(string value, int number);
    }

Sample invocation:

    string result = await client.PostString("foo", 5);

Generated body:

    { "value": "foo", "number": 5 }

Note that once you have more than one body parameter, it is assumed that you want the serialized JSON to be an object
composed of those parameters.  Therefore, you can omit the `[Object]` attribute when you have multiple body parameters.

### Serializing as plain text

Alternatively, perhaps you didn't want those double quotes in there in the first example?  You just want to post a raw 
string as-is without any interference from serialization?  To do this, decorate your method with the `[Text]` attribute, 
indicating the POST should be `text/plain` and that the contents will be supplied by the argument converted to a string 
(or not converted at all if it's already a string).

    public interface ISampleApi
    {
        [Post, Text]
        Task<string> PostString(string value);
    }

With the addition of the `[Text]` attribute, the `value` parameter is sent as-is such that the HTTP body of the request is:

    foo

## Paths

Up until now, all of our requests have been made directly against "http://someserver.com".  The vast majority of the time, you
will want to append extra path information to the url.  To demonstrate this, we'll imagine a standard REST API with CRUD
operations made available for a type called `User`:

    [Path("users")]
    public interface IUserApi
    {
        [Get("{id}")]
        Task<User> Get(int id);

        [Post]
        Task<int> Post(User user);

        [Put("{id}")]
        Task Put(int id, User user);

        [Delete("{id}")]
        Task Delete(int id);
    }

Assuming each of these endpoints were called like so:

    var user = await api.GetById(1);
    var newId = await api.Post(new User());
    await api.Put(2, new User());
    await api.Delete(3);

Then the following URLs would be used respectively:

    http://someserver.com/users/1
    http://someserver.com/users
    http://someserver.com/users/2
    http://someserver.com/users/3

As you can see, the API as a whole may (optionally) provide a path prefix (`[Path("users")]`).  Second, each individual
endpoint can specify its path. Furthermore, you can reference your arguments by parameter name by enclosing the name in 
braces.  This allows your paths to easily contain dynamic content.

### Query Strings

You can also include query strings in your endpoint paths.  Let's add a new endpoint for getting all the users with 
some optional filtering via the query string:

    [Get("?ids={ids}&firstName={firstName}&lastName={lastName}")]
    Task<User[]> Find(int[] ids = null, string firstName = null, string lastName = null);

First some examples, than a detailed explanation:

    api.Find(ids: new[] { 1, 3 });
    api.Find(firstName: "John", lastName: "Doe");
    api.Find();

These will produce, respectively:

    http://someserver.com/users?ids=1&ids=3
    http://someserver.com/users?firstName=John&lastName=Doe
    http://someserver.com/users

#### Comma-separated string for arrays

The default behavior for an array argument in the query string is to use multiple `name=value` pairs.  (You can override 
this to use a comma separated string instead by annotating your API with 

    [TypeConverter(ArrayAsCommaSeparatedStringConverter, TypeConversionContext.Query)]
    
Specifying the `TypeConversionContext` prevents the converter from being used in contexts other than the query string.

## Parameter Types

The type of a parameter for your method can be important if they are one of the following types:

* **`Stream`**  
  When the parameter is a `Stream`, it will be consumed upon invocation as the body of the HTTP request.  Useful for 
  things like uploading files without having to buffer the entire payload into memory.

* **`byte[]`**  
  Similar to `Stream` as defined above but the HTTP body is simply this raw byte array.

* **`Func<Stream, Task>`**  
  When the parameter is of this type, the idea is that you provide a method that consumes a stream asynchronously.  In 
  other words, this allows you to access the *response* as a `Stream` and completely handle it in the context of the 
  method such that when the invocation to the API is complete, it everything can be disposed of immediately.  This is 
  why it's a *parameter* of type `Func<Stream, Task>` rather a *return* type of `Stream`.  If we implemented it as a 
  return type, then we couldn't dispose of the `HttpClient` upon completion of the invocation of the method.
  
* **`Action<HttpApiRequest>`**  
  This allows you to instrument the request before sending it to the server.  You can use this to either *completely* 
  define the parameters to the backend endpoint, or you can use this to simply tweak the nature of the request *in
  addition* to also naturally filling in the request as usual.  For example, you could modify the URL, change up the 
  body, etc.
  
* **`HttpBody`**  
  When you provide a parameter of this type, the entire body of the HTTP request will use this value directly.  There
  are various subclasses of `HttpBody` (such as `JsonHttpBody` and `StreamHttpBody`) that allows you to provide data 
  of different kinds.
  
## Response Types

Similar to the above, if the return type of your method is one of the following types, then it is handled specially as 
defined below:

* **`byte[]`**  
  The response body is returned as a raw byte array.
  
* **`HttpApiResponse`** 
  An `HttpApiResponse` describes the HTTP response, including its headers and body.  You can further interrogate the body 
  by checking its type and casting accordingly (or by using an `IHttpBodyVisitor`).
  
* **`HttpBody`**
  Similar to the above that returns an `HttpApiResponse`, except this provides you with only the body, so you wouldn't be
  able to access the headers, etc.