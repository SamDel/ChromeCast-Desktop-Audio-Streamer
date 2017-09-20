using System;
using Rssdp;
using ChromeCast.Desktop.AudioStreamer.Discover.Interfaces;
using System.Net;
using System.Net.Sockets;

namespace ChromeCast.Desktop.AudioStreamer.Discover
{
    public class DiscoverServiceSSDP : IDiscoverServiceSSDP
    {
        private const string ChromeCastUpnpDeviceType = "urn:dial-multiscreen-org:device:dial:1";
        private Action<DiscoveredSsdpDevice, SsdpDevice> onDiscovered;
        private Action updateCounter;

        public void Discover(Action<DiscoveredSsdpDevice, SsdpDevice> onDiscoveredIn, Action updateCounterIn)
        {
            onDiscovered = onDiscoveredIn;
            updateCounter = updateCounterIn;

            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = GetIp4Address(ipHostInfo);
            using (var deviceLocator =
                new SsdpDeviceLocator(
                    communicationsServer: new Rssdp.Infrastructure.SsdpCommunicationsServer(
                        new SocketFactory(ipAddress: ipAddress.ToString())
                    )
                ))
            {
                deviceLocator.NotificationFilter = ChromeCastUpnpDeviceType;
                deviceLocator.DeviceAvailable += OnDeviceAvailable;
                deviceLocator.SearchAsync();
            }
        }

        private async void OnDeviceAvailable(object sender, DeviceAvailableEventArgs e)
        {
            var fullDevice = await e.DiscoveredDevice.GetDeviceInfo();
            onDiscovered?.Invoke(e.DiscoveredDevice, fullDevice);
            updateCounter?.Invoke();
        }

        private IPAddress GetIp4Address(IPHostEntry ipHostInfo)
        {
            var ipAddress = ipHostInfo.AddressList[0];
            foreach (var address in ipHostInfo.AddressList)
            {
                if (address.AddressFamily.Equals(AddressFamily.InterNetwork))
                {
                    ipAddress = address;
                }
            }

            return ipAddress;
        }
    }
}
