using System;
using NLog;
using NUnit.Framework;

namespace More.NLog.Tests
{
    [TestFixture]
    public sealed class Tests
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        [Test]
        public void Test()
        {
            log.Trace("This is Trace message.");
            log.Debug("This is Debug message.");
            log.Info ("This is Information message.");
            log.Warn ("This is Warning message.");
            log.Error(new OutOfMemoryException(), "This is Error message.");
            log.Fatal(new OutOfMemoryException(), "This is Fatal message.");
        }
    }
}
