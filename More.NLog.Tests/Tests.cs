using System;
using NLog;

namespace More.NLog.Tests
{
    public sealed class Tests
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();

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
