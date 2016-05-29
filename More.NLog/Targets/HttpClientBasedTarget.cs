using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NLog;
using NLog.Common;
using NLog.Targets;

namespace More.NLog.Targets
{
    public abstract class HttpClientBasedTarget : TargetWithLayout
    {
        protected readonly HttpClient Client = new HttpClient();

        protected override void Write(LogEventInfo logEvent)
        {
            throw new NotSupportedException("Synchronous write operation is not supported.");
        }

        protected override void Write(AsyncLogEventInfo info)
        {
            try
            {
                WriteAsync(info.LogEvent).ContinueWith(t =>
                {
                    var exception = t.Exception?.GetBaseException();

                    if (exception != null)
                    {
                        InternalLogger.Error(exception, "WriteAsync error.");
                    }

                    info.Continuation(exception);
                });
            }
            catch (Exception ex)
            {
                InternalLogger.Error(ex, "WriteAsync error.");
                info.Continuation(ex);
            }
        }

        private async Task WriteAsync(LogEventInfo logEvent)
        {
            var result = await Client.PostAsJsonAsync(Url, GetContent(logEvent));

            InternalLogger.Debug("Response from {0}: {1}", Url, result);

            if (!result.IsSuccessStatusCode)
            {
                throw new HttpResponseException(result);
            }
        }

        protected abstract string Url { get; }

        protected abstract IDictionary<string, string> GetContent(LogEventInfo logEvent);

        protected override void CloseTarget()
        {
            Client.Dispose();
            base.CloseTarget();
        }
    }
}