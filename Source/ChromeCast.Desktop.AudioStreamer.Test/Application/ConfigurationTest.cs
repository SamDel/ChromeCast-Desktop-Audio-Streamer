using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChromeCast.Desktop.AudioStreamer.Application;

namespace ChromeCast.Desktop.AudioStreamer.Test.Application
{
    [TestClass]
    public class ConfigurationTest
    {
        AutoResetEvent asyncEvent;
        string ipAddressesDevices;

        [TestMethod]
        public void TestMethod1()
        {
            asyncEvent = new AutoResetEvent(false);

            var configuration = new Configuration();
            configuration.Load(ConfigurationCallback, null);

            asyncEvent.WaitOne(100);

            Assert.AreEqual(string.Empty, ipAddressesDevices);
        }

        private void ConfigurationCallback(string ipAddressesDevicesIn, string ignoreIpAddressesDevicesIn, bool showLagControl)
        {
            ipAddressesDevices = ipAddressesDevicesIn;

            asyncEvent.Set();
        }
    }
}
