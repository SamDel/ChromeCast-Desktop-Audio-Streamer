using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChromeCast.Desktop.AudioStreamer.Application;

namespace ChromeCast.Desktop.AudioStreamer.Test.Application
{
    [TestClass]
    public class ConfigurationTest
    {
        AutoResetEvent asyncEvent;
        bool useShortCuts;
        bool boolShowLog;
        bool showLag;
        int lagValue;
        bool autoStart;
        string ipAddressesDevices;

        [TestMethod]
        public void TestMethod1()
        {
            asyncEvent = new AutoResetEvent(false);

            var configuration = new Configuration();
            configuration.Load(ConfigurationCallback);

            asyncEvent.WaitOne(100);

            Assert.IsFalse(useShortCuts);
            Assert.IsFalse(boolShowLog);
            Assert.IsFalse(showLag);
            Assert.AreEqual(1000, lagValue);
            Assert.AreEqual(string.Empty, ipAddressesDevices);
        }

        private void ConfigurationCallback(bool useShortCutsIn, bool boolShowLogIn, bool showLagIn, int lagValueIn, bool autoStartIn, string ipAddressesDevicesIn)
        {
            useShortCuts = useShortCutsIn;
            boolShowLog = boolShowLogIn;
            showLag = showLagIn;
            lagValue = lagValueIn;
            autoStart = autoStartIn;
            ipAddressesDevices = ipAddressesDevicesIn;

            asyncEvent.Set();
        }
    }
}
