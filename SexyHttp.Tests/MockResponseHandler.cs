﻿using System;
using System.Threading.Tasks;

namespace SexyHttp.Tests
{
    public class MockResponseHandler : IHttpResponseHandler
    {
        public object Response { get; set; }

        private readonly Func<HttpApiResponse, object> responseFactory;

        public MockResponseHandler() : this(_ => null)
        {
        }

        public MockResponseHandler(object response = null) : this(_ => response)
        {
        }

        public MockResponseHandler(Func<HttpApiResponse, object> responseFactory)
        {
            this.responseFactory = responseFactory;
        }

        public Task<object> HandleResponse(HttpApiResponse response) 
        {
            return Task.FromResult(responseFactory(response));
        }
    }
}