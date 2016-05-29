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
        private static readonly Dictionary<int, string> Emojies = new Dictionary<int, string>
        {
            {LogLevel.Trace.Ordinal, char.ConvertFromUtf32(0x1F4AC)},
            {LogLevel.Debug.Ordinal, char.ConvertFromUtf32(0x1F47B)},
            {LogLevel.Info .Ordinal, char.ConvertFromUtf32(0x2139 )},
            {LogLevel.Warn .Ordinal, char.ConvertFromUtf32(0x26A0 )},
            {LogLevel.Error.Ordinal, char.ConvertFromUtf32(0x1F6AB)},
            {LogLevel.Fatal.Ordinal, char.ConvertFromUtf32(0x26d4 )}
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

            return Emojies.TryGetValue(logEvent.Level.Ordinal, out emoji) ? emoji : string.Empty;
        }

        protected override string Transform(string text) => text;
    }
}
