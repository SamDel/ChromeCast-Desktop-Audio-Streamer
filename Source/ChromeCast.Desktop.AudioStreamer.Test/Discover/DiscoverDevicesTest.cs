using System.Threading;
using Rssdp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChromeCast.Desktop.AudioStreamer.Discover;

namespace ChromeCast.Desktop.AudioStreamer.Test.Discover
{
    [TestClass]
    public class DiscoverDevicesTest
    {
        AutoResetEvent asyncEvent;
        DiscoveredSsdpDevice discoveredSsdpDevice;

        [TestMethod]
        public void TestDiscover()
        {
            asyncEvent = new AutoResetEvent(false);

            var discoverDevice = new DiscoverDevices(new DiscoverServiceSSDP());
            discoverDevice.Discover(DiscoverCallBack);

            asyncEvent.WaitOne(30000);

            Assert.IsNotNull(discoveredSsdpDevice, "Device not found within 30 seconds.");
        }

        private void DiscoverCallBack(DiscoveredSsdpDevice discoveredSsdpDeviceIn, SsdpDevice ssdpDevice, ushort port)
        {
            discoveredSsdpDevice = discoveredSsdpDeviceIn;
            asyncEvent.Set();
        }
    }
}
