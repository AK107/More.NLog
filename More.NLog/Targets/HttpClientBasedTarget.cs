using System;
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

        protected override void Write(AsyncLogEventInfo info)
        {
            try
            {
                Send(info.LogEvent).Wait();
            }
            catch (Exception ex)
            {
                info.Continuation(ex);
            }
        }

        protected abstract Task Send(LogEventInfo logEvent);

        protected override void CloseTarget()
        {
            base.CloseTarget();
            Client.Dispose();
        }
    }
}