using NLog;
using NUnit.Framework;

namespace More.NLog.Tests
{
    [TestFixture]
    public sealed class Tests
    {
        [Test]
        public void Test()
        {
            var log = LogManager.GetCurrentClassLogger();

            log.Trace("This is Trace message.");
            log.Debug("This is Debug message.");
            log.Info ("This is Information message.");
            log.Warn ("This is Warning message.");
            log.Error("This is Error message.");
            log.Fatal("This is Fatal message.");

            LogManager.Flush();
        }
    }
}
