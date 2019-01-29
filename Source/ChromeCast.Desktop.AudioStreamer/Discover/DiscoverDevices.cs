using System;
using ChromeCast.Desktop.AudioStreamer.Discover.Interfaces;
using Tmds.MDns;
using System.Linq;
using System.Timers;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ChromeCast.Desktop.AudioStreamer.Discover
{
    public class DiscoverDevices : IDiscoverDevices
    {
        public const int Interval = 2000;
        public const int MaxNumberOfTries = 15;
        private Action<DiscoveredDevice> onDiscovered;
        private int numberDiscovered;
        private List<DiscoveredDevice> discoveredDevices;
        private Timer timer;

        public DiscoverDevices()
        {
        }

        public void Discover(Action<DiscoveredDevice> onDiscoveredIn)
        {
            onDiscovered = onDiscoveredIn;
            discoveredDevices = new List<DiscoveredDevice>();
            timer = new Timer
            {
                Interval = 500,
                Enabled = true
            };
            timer.Elapsed += new ElapsedEventHandler(OnAddDevice);
            timer.Start();

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
            var discoveredDevice = new DiscoveredDevice
            {
                IPAddress = e.Announcement.Addresses[0].ToString(),
                Protocol = e.Announcement.Type,
                Port = e.Announcement.Port,
                Name = e.Announcement.Txt.Where(x => x.ToString().StartsWith("fn=")).FirstOrDefault()?.Replace("fn=", ""),
                Headers = JsonConvert.SerializeObject(e.Announcement.Txt),
                Usn = e.Announcement.Hostname
            };

            if (discoveredDevice.Name != null 
                && discoveredDevice.Usn != null 
                && discoveredDevice.Headers != null
                && (discoveredDevice.Protocol.IndexOf("_googlecast._tcp") >= 0 
                    || discoveredDevice.Protocol.IndexOf("_googlezone._tcp") >= 0))
            {
                discoveredDevices.Add(discoveredDevice);
            }
        }

        private void OnAddDevice(object sender, ElapsedEventArgs e)
        {
            lock (discoveredDevices)
            {
                if (discoveredDevices.Count > 0)
                {
                    var discoveredDevice = discoveredDevices[0];
                    onDiscovered(discoveredDevice);
                    discoveredDevices.RemoveAt(0);
                }
            }
        }
    }
}