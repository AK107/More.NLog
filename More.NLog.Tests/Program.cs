using NLog;

namespace More.NLog.Tests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new Tests().Test();

            LogManager.Flush();
        }
    }
}
