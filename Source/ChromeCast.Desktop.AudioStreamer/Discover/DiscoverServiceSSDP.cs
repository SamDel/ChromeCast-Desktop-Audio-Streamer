using Rssdp;
using ChromeCast.Desktop.AudioStreamer.Application;

namespace ChromeCast.Desktop.AudioStreamer.Discover
{
    public class DiscoverServiceSSDP
    {
        private const string ChromeCastUpnpDeviceType = "urn:dial-multiscreen-org:device:dial:1";
        private ApplicationLogic application;

        public DiscoverServiceSSDP(ApplicationLogic app)
        {
            application = app;
        }

        public void Discover()
        {
            using (var deviceLocator = new SsdpDeviceLocator())
            {
                deviceLocator.NotificationFilter = ChromeCastUpnpDeviceType;
                deviceLocator.DeviceAvailable += OnDeviceAvailable;
                deviceLocator.SearchAsync();
            }
        }

        private async void OnDeviceAvailable(object sender, DeviceAvailableEventArgs e)
        {
            var fullDevice = await e.DiscoveredDevice.GetDeviceInfo();
            application.OnDeviceAvailable(e.DiscoveredDevice, fullDevice);
        }
    }
}
