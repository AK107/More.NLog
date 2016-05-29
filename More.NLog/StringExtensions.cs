namespace More.NLog
{
    static class StringExtensions
    {
        public static string Cut(this string value, int maxLen)
        {
            return value.Length > maxLen ? value.Substring(0, maxLen) : value;
        }
    }
}
