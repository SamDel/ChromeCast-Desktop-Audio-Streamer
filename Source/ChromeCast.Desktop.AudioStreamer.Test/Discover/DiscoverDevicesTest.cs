using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChromeCast.Desktop.AudioStreamer.Discover;

namespace ChromeCast.Desktop.AudioStreamer.Test.Discover
{
    [TestClass]
    public class DiscoverDevicesTest
    {
        AutoResetEvent asyncEvent;
        DiscoveredDevice discoveredDevice;

        [TestMethod]
        public void TestDiscover()
        {
            asyncEvent = new AutoResetEvent(false);

            var discoverDevice = new DiscoverDevices();
            discoverDevice.Discover(DiscoverCallBack);

            asyncEvent.WaitOne(30000);

            Assert.IsNotNull(discoveredDevice, "Device not found within 30 seconds.");
        }

        private void DiscoverCallBack(DiscoveredDevice discoveredDevice)
        {
            this.discoveredDevice = discoveredDevice;
            asyncEvent.Set();
        }
    }
}
