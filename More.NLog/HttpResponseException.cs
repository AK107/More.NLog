using System;
using System.Net.Http;

namespace More.NLog
{
    public sealed class HttpResponseException : Exception
    {
        public HttpResponseMessage Result { get; }

        public HttpResponseException(HttpResponseMessage result)
            : base($"{result.StatusCode} ({(int) result.StatusCode}), {result.ReasonPhrase}.")
        {
            Result = result;
        }
    }
}