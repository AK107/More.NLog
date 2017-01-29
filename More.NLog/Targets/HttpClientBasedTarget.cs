using System;
using System.Collections.Concurrent;
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
        private readonly ConcurrentDictionary<AsyncLogEventInfo, Task> pendingEvents = new ConcurrentDictionary<AsyncLogEventInfo, Task>();

        protected readonly HttpClient Client = new HttpClient
        (
#if NETSTANDARD
            new HttpClientHandler { DefaultProxyCredentials = System.Net.CredentialCache.DefaultCredentials }
#endif
        );

        protected sealed override void Write(AsyncLogEventInfo info)
        {
            var task = WriteAsync(info);

            InternalLogger.Trace("Add pending event: {0}.", info.LogEvent.SequenceID);

            pendingEvents.TryAdd(info, task);

            task.ContinueWith(x =>
            {
                InternalLogger.Trace("Remove pending event: {0}.", info.LogEvent.SequenceID);

                Task tmp;
                pendingEvents.TryRemove(info, out tmp);
            }, 
            TaskContinuationOptions.ExecuteSynchronously);
        }

        private async Task WriteAsync(AsyncLogEventInfo info)
        {
            try
            {
                InternalLogger.Debug("Async post event: {0}.", info.LogEvent.SequenceID);

                var result = await Client.PostAsync(Url, new JsonContent(GetContent(info.LogEvent)));

                InternalLogger.Debug("Response of event {0} from '{1}': {2}", info.LogEvent.SequenceID, Url, result);

                result.EnsureSuccessStatusCode();

                info.Continuation(null);
            }
            catch (Exception ex)
            {
                InternalLogger.Error(ex, "Async write error of event: {0}.", info.LogEvent.SequenceID);
                info.Continuation(ex);
            }
        }

        protected override void FlushAsync(AsyncContinuation asyncContinuation)
        {
            Task.WhenAll(pendingEvents.Values)
                .ContinueWith(x => asyncContinuation(x.Exception), TaskContinuationOptions.ExecuteSynchronously);
        }

        protected sealed override void CloseTarget()
        {
            base.CloseTarget();
            Client.Dispose();
        }

        protected abstract string Url { get; }

        protected abstract IDictionary<string, object> GetContent(LogEventInfo logEvent);
    }
}