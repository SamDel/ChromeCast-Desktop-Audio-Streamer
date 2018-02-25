using System;
using System.Timers;
using Rssdp;
using ChromeCast.Desktop.AudioStreamer.Discover.Interfaces;
using System.Linq;
using Zeroconf;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace ChromeCast.Desktop.AudioStreamer.Discover
{
    public class DiscoverDevices : IDiscoverDevices
    {
        public const int Interval = 2000;
        public const int MaxNumberOfTries = 15;
        private IDiscoverServiceSSDP discoverServiceSSDP;
        private Action<DiscoveredSsdpDevice, SsdpDevice> onDiscovered;
        private Timer timer;
        private int numberOfTries;
        private int numberDiscovered;

        public DiscoverDevices(IDiscoverServiceSSDP discoverServiceSSDPIn)
        {
            discoverServiceSSDP = discoverServiceSSDPIn;
        }

        public void Discover(Action<DiscoveredSsdpDevice, SsdpDevice> onDiscoveredIn)
        {
            onDiscovered = onDiscoveredIn;

            // SSDP search
            discoverServiceSSDP.Discover(onDiscovered, UpdateCounter);

            // MDNS search
            MdnsSearch();

            numberDiscovered = 0;
        }

        public void UpdateCounter()
        {
            numberDiscovered++;
        }

        public async void MdnsSearch()
        {
            ILookup<string, string> domains = await ZeroconfResolver.BrowseDomainsAsync(scanTime: new TimeSpan(1000000000), retries: 5, callback: mdnsCallback);
        }

        private void mdnsCallback(string protocol, string ipAddress)
        {
            if (protocol.StartsWith("_googlecast"))
            {
                onDiscovered(
                    new DiscoveredSsdpDevice { DescriptionLocation = new Uri($"http://{ipAddress}"), Usn = ipAddress },
                   new SsdpRootDevice { FriendlyName = GetDeviceFriendlyNameAsync(ipAddress).Result }
                );
            }
        }

        private async Task<string> GetDeviceFriendlyNameAsync(string ipAddress)
        {
            var friendlyName = "";
            try
            {
                var http = new HttpClient();
                var responce = await http.GetAsync($"http://{ipAddress}:8008/setup/eureka_info?options=detail");
                var json = await responce.Content.ReadAsStringAsync();
                var info = JsonConvert.DeserializeObject<EurekaInfo>(json);
                friendlyName = info.Name;
            }
            catch (Exception)
            {
                friendlyName = ipAddress;
            }
            return friendlyName;
        }
    }

    public class EurekaInfo
    {
        public string Name { get; set; }
    }
}