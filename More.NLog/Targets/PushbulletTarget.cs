using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

namespace More.NLog.Targets
{
    [Target("Pushbullet")]
    public sealed class PushbulletTarget : HttpClientBasedTarget
    {
        private const string ApiUrl            = "https://api.pushbullet.com/v2/pushes";
        private const string AccessTokenHeader = "Access-Token";

        private const int MaxTitleLength       = 200;
        private const int MaxTextLength        = 4096;

        [RequiredParameter]
        public string Token
        {
            get
            {
                return Client.DefaultRequestHeaders.GetValues(AccessTokenHeader)?.Single();
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

        protected override async Task Send(LogEventInfo logEvent)
        {
            var dictionary = new Dictionary<string, object>
            {
                {"type" , "note"},
                {"title",  Title.Render(logEvent).Cut(MaxTitleLength)},
                {"body" , Layout.Render(logEvent).Cut(MaxTextLength)},
            };

            var result = await Client.PostAsJsonAsync(ApiUrl, dictionary);

            InternalLogger.Debug(result.ToString());

            if (!result.IsSuccessStatusCode)
            {
                throw new HttpResponseException(result);
            }

        }
    }
}
