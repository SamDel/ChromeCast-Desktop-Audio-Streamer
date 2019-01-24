using System;
using Rssdp;
using ChromeCast.Desktop.AudioStreamer.Discover.Interfaces;
using System.Net;

namespace ChromeCast.Desktop.AudioStreamer.Discover
{
    public class DiscoverServiceSSDP : IDiscoverServiceSSDP
    {
        private const string ChromeCastUpnpDeviceType = "urn:dial-multiscreen-org:device:dial:1";
        private Action<DiscoveredSsdpDevice, SsdpDevice, ushort> onDiscovered;
        private Action updateCounter;

        public void Discover(Action<DiscoveredSsdpDevice, SsdpDevice, ushort> onDiscoveredIn, Action updateCounterIn)
        {
            onDiscovered = onDiscoveredIn;
            updateCounter = updateCounterIn;

            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddresses = GetIpAddresses(ipHostInfo);
            foreach (var ipAddress in ipAddresses)
            {
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
        }

        private async void OnDeviceAvailable(object sender, DeviceAvailableEventArgs e)
        {
            var fullDevice = await e.DiscoveredDevice.GetDeviceInfo();
            //TODO: Port = 8009, are groups discovered by SSDP?
            onDiscovered?.Invoke(e.DiscoveredDevice, fullDevice, 8009);
            updateCounter?.Invoke();
        }

        private IPAddress[] GetIpAddresses(IPHostEntry ipHostInfo)
        {
            return ipHostInfo.AddressList;
        }
    }
}
