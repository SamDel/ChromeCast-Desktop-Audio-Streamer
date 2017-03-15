using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChromeCast.Desktop.AudioStreamer.Application;

namespace ChromeCast.Desktop.AudioStreamer.Test.Application
{
    [TestClass]
    public class DeviceStatusTimerTest
    {
        AutoResetEvent asyncEvent;
        private bool callbackCalled = false;

        [TestMethod]
        public void TestDeviceStatusTimerCallback()
        {
            asyncEvent = new AutoResetEvent(false);

            var deviceStatusTimer = new DeviceStatusTimer();
            deviceStatusTimer.StartPollingDevice(DeviceStatusTimerCallback);

            asyncEvent.WaitOne(DeviceStatusTimer.Interval * 2);

            Assert.IsTrue(callbackCalled, "Callback is not called.");
        }

        private void DeviceStatusTimerCallback()
        {
            callbackCalled = true;
            asyncEvent.Set();
        }
    }
}
