using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

namespace More.NLog.Targets
{
    [Target("Pushbullet")]
    public sealed class PushbulletTarget : HttpClientBasedTarget
    {
        private const string PushbulletApiUrl  = "https://api.pushbullet.com/v2/pushes";
        private const string AccessTokenHeader = "Access-Token";

        private const int    MaxTitleLength    = 200;
        private const int    MaxTextLength     = 4096;

        [RequiredParameter]
        public string Token
        {
            get
            {
                IEnumerable<string> values;
                return Client.DefaultRequestHeaders.TryGetValues(AccessTokenHeader, out values) ? values.Single() : null;
            }
            set
            {
                Client.DefaultRequestHeaders.Remove(AccessTokenHeader);
                Client.DefaultRequestHeaders.Add(AccessTokenHeader, value);
            }
        }

        [DefaultValue("Message from NLog on ${machinename}")]
        public Layout Title { get; set; }

        public PushbulletTarget()
        {
            Title = "Message from NLog on ${machinename}";
        }

        protected override string Url => PushbulletApiUrl;

        protected override IDictionary<string, object> GetContent(LogEventInfo logEvent)
        {
            return new Dictionary<string, object>
            {
                { "type" , "note" },
                { "title", Title .Render(logEvent).Cut(MaxTitleLength) },
                { "body" , Layout.Render(logEvent).Cut(MaxTextLength)  }
            };
        }
    }
}
