using System;
using System.Net.Http;

namespace More.NLog
{
    public sealed class HttpResponseException : Exception
    {
        private readonly HttpResponseMessage result;

        public HttpResponseException(HttpResponseMessage result)
            : base($"Http response exception: {result.StatusCode}.")
        {
            this.result = result;
        }
    }
}