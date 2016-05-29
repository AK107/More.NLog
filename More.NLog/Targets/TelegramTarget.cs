using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Targets;

namespace More.NLog.Targets
{
    [Target("Telegram")]
    public sealed class TelegramTarget : HttpClientBasedTarget
    {
        private const string ApiUrl = "https://api.telegram.org/bot{0}/sendMessage";

        private const int MaxTextLength = 4096;

        [RequiredParameter]
        public string Token { get; set; }

        [RequiredParameter]
        public string ChatId { get; set; }

        protected override async Task Send(LogEventInfo logEvent)
        {
            var dictionary = new Dictionary<string, string>
            {
                {"chat_id", ChatId },
                {"text"   ,  Layout.Render(logEvent).Cut(MaxTextLength)},
            };

            var result = await Client.PostAsJsonAsync(string.Format(ApiUrl, Token), dictionary);

            InternalLogger.Debug(result.ToString());

            if (!result.IsSuccessStatusCode)
            {
                throw new HttpResponseException(result);
            }

        }
    }
}
