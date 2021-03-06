﻿using System.Threading.Tasks;
using SexyHttp.HttpBodies;

namespace SexyHttp.ResponseHandlers
{
    public class StringResponseHandler : HttpResponseHandler
    {
        protected override Task<object> ProvideResult(HttpApiRequest request, HttpApiResponse response)
        {
            return Task.FromResult<object>(((StringHttpBody)response.Body).Text);
        }
    }
}
