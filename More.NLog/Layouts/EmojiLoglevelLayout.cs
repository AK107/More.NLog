using System.Collections.Generic;
using System.ComponentModel;
using NLog;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.LayoutRenderers.Wrappers;

namespace More.NLog.Layouts
{
    [LayoutRenderer("emoji")]
    [AmbientProperty("Emoji")]
    [ThreadAgnostic]
    public class EmojiLogLevelLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        private static readonly Dictionary<LogLevel, string> Emojies = new Dictionary<LogLevel, string>
        {
            { LogLevel.Trace, "💬" },
            { LogLevel.Debug, "👻" },
            { LogLevel.Info , "ℹ" },
            { LogLevel.Warn , "⚠" },
            { LogLevel.Error, "🚫" },
            { LogLevel.Fatal, "⛔" }
        };

        [DefaultValue(true)]
        public bool Emoji { get; set; }

        public EmojiLogLevelLayoutRendererWrapper()
        {
            Emoji = true;
        }

        protected override string RenderInner(LogEventInfo logEvent)
        {
            if (!Emoji)
            {
                return base.RenderInner(logEvent);
            }

            string emoji;

            return Emojies.TryGetValue(logEvent.Level, out emoji) ? emoji : string.Empty;
        }

        protected override string Transform(string text) => text;
    }
}
