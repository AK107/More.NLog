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

        protected readonly HttpClient Client = new HttpClient();

        protected sealed override void Write(AsyncLogEventInfo info)
        {
            WriteAsync(info);
        }

        private async void WriteAsync(AsyncLogEventInfo info)
        {
            var task = DoWriteAsync(info);

            InternalLogger.Trace("Add pending event: {0}.", info.LogEvent.SequenceID);

            pendingEvents.TryAdd(info, task);

            Exception exception = null;

            try
            {
                await task;
            }
            catch (Exception ex)
            {
                InternalLogger.Error(ex, "Async write error of event: {0}.", info.LogEvent.SequenceID);
                exception = ex;
            }

            InternalLogger.Trace("Remove pending event: {0}.", info.LogEvent.SequenceID);

            Task tmp;
            pendingEvents.TryRemove(info, out tmp);

            info.Continuation(exception);
        }

        private async Task DoWriteAsync(AsyncLogEventInfo info)
        {
            InternalLogger.Debug("Async post event: {0}.", info.LogEvent.SequenceID);

            var result = await Client.PostAsync(Url, new JsonContent(GetContent(info.LogEvent)));

            InternalLogger.Debug("Response of event {0} from '{1}': {2}", info.LogEvent.SequenceID, Url, result);

            result.EnsureSuccessStatusCode();
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