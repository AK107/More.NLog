using System;
using System.Collections.Generic;
using System.Net.Http;
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
            WriteAsync(info);
        }

        private async void WriteAsync(AsyncLogEventInfo info)
        {
            try
            {
                var result = await Client.PostAsync(Url, new JsonContent(GetContent(info.LogEvent)));

                InternalLogger.Debug("Response from {0}: {1}", Url, result);

                if (!result.IsSuccessStatusCode)
                {
                    throw new HttpResponseException(result);
                }

                info.Continuation(null);
            }
            catch (Exception ex)
            {
                InternalLogger.Error(ex, "WriteAsync error.");
                info.Continuation(ex);
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