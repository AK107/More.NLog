using System.Collections.Generic;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace More.NLog.Targets
{
    [Target("Telegram")]
    public sealed class TelegramTarget : HttpClientBasedTarget
    {
        private const int MaxTextLength = 4096;

        [RequiredParameter]
        public string Token { get; set; }

        [RequiredParameter]
        public string ChatId { get; set; }

        protected override string Url => $"https://api.telegram.org/bot{Token}/sendMessage";

        protected override IDictionary<string, string> GetContent(LogEventInfo logEvent)
        {
            return new Dictionary<string, string>
            {
                {"chat_id", ChatId },
                {"text"   , Layout.Render(logEvent).Cut(MaxTextLength)},
            };
        }
    }
}
