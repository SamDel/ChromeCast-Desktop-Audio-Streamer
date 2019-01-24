using System;
using Rssdp;
using ChromeCast.Desktop.AudioStreamer.Discover.Interfaces;
using Tmds.MDns;
using System.Linq;

namespace ChromeCast.Desktop.AudioStreamer.Discover
{
    public class DiscoverDevices : IDiscoverDevices
    {
        public const int Interval = 2000;
        public const int MaxNumberOfTries = 15;
        private IDiscoverServiceSSDP discoverServiceSSDP;
        private Action<DiscoveredSsdpDevice, SsdpDevice, ushort> onDiscovered;
        private int numberDiscovered;

        public DiscoverDevices(IDiscoverServiceSSDP discoverServiceSSDPIn)
        {
            discoverServiceSSDP = discoverServiceSSDPIn;
        }

        public void Discover(Action<DiscoveredSsdpDevice, SsdpDevice, ushort> onDiscoveredIn)
        {
            onDiscovered = onDiscoveredIn;

            // MDNS search
            MdnsSearch();

            numberDiscovered = 0;
        }

        public void UpdateCounter()
        {
            numberDiscovered++;
        }

        public void MdnsSearch()
        {
            string serviceType = "_googlecast._tcp";
            ServiceBrowser serviceBrowser = new ServiceBrowser();
            serviceBrowser.ServiceAdded += OnServiceAdded;
            serviceBrowser.ServiceRemoved += OnServiceRemoved;
            serviceBrowser.ServiceChanged += OnServiceChanged;
            serviceBrowser.StartBrowse(serviceType);

            string serviceTypeEmbedded = "_googlezone._tcp";
            ServiceBrowser serviceBrowserEmbedded = new ServiceBrowser();
            serviceBrowserEmbedded.ServiceAdded += OnServiceAdded;
            serviceBrowserEmbedded.ServiceRemoved += OnServiceRemoved;
            serviceBrowserEmbedded.ServiceChanged += OnServiceChanged;
            serviceBrowserEmbedded.StartBrowse(serviceTypeEmbedded);
        }

        private void OnServiceChanged(object sender, ServiceAnnouncementEventArgs e)
        {
        }

        private void OnServiceRemoved(object sender, ServiceAnnouncementEventArgs e)
        {
        }

        private void OnServiceAdded(object sender, ServiceAnnouncementEventArgs e)
        {
            var protocol = e.Announcement.Type;
            var ipAddress = e.Announcement.Addresses[0].ToString();
            var port = e.Announcement.Port;
            var name = e.Announcement.Txt.Where(x => x.ToString().StartsWith("fn=")).FirstOrDefault()?.Replace("fn=", "");

            if (name != null)
            {
                MdnsCallback(protocol, ipAddress, port, name);
            }
        }

        private void MdnsCallback(string protocol, string ipAddress, ushort port, string name)
        {
            if (protocol.IndexOf("_googlecast._tcp") >= 0 || protocol.IndexOf("_googlezone._tcp") >= 0)
            {
                onDiscovered(
                    new DiscoveredSsdpDevice { DescriptionLocation = new Uri($"http://{ipAddress}"), Usn = ipAddress },
                    new SsdpRootDevice { FriendlyName = name },
                    port
                );
            }
        }
    }
}