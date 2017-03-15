using System.Threading;
using ChromeCast.Desktop.AudioStreamer.Application;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChromeCast.Desktop.AudioStreamer.Test
{
    [TestClass]
    public class LoggerTest
    {
        AutoResetEvent asyncEvent;
        string loggedMessage;

        [TestMethod]
        public void TestLogCallback()
        {
            asyncEvent = new AutoResetEvent(false);

            var logMessage = "test 123";
            var logger = new Logger();
            logger.SetCallback(LogCallBack);
            logger.Log(logMessage);

            asyncEvent.WaitOne(100);

            Assert.AreEqual(logMessage, loggedMessage);
        }

        private void LogCallBack(string message)
        {
            loggedMessage = message;
            asyncEvent.Set();
        }
    }
}
