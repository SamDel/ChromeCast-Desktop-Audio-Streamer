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
        private const string serviceType = "_googlecast._tcp";
        private const string serviceTypeEmbedded = "_googlezone._tcp";
        private Action<DiscoveredDevice> onDiscovered;
        private List<DiscoveredDevice> discoveredDevices;
        private Timer timer;

        public DiscoverDevices()
        {
        }

        /// <summary>
        /// Start discovering devices.
        /// </summary>
        /// <param name="onDiscoveredIn">callback for when a device is discovered</param>
        public void Discover(Action<DiscoveredDevice> onDiscoveredIn)
        {
            if (onDiscoveredIn == null)
                return;

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
        }

        /// <summary>
        /// Do a scan on the network.
        /// </summary>
        public void MdnsSearch()
        {
            ServiceBrowser serviceBrowser = new ServiceBrowser();
            serviceBrowser.ServiceAdded += OnServiceAdded;
            serviceBrowser.ServiceRemoved += OnServiceRemoved;
            serviceBrowser.ServiceChanged += OnServiceChanged;
            serviceBrowser.StartBrowse(serviceType);

            ServiceBrowser serviceBrowserEmbedded = new ServiceBrowser();
            serviceBrowserEmbedded.ServiceAdded += OnServiceAdded;
            serviceBrowserEmbedded.ServiceRemoved += OnServiceRemoved;
            serviceBrowserEmbedded.ServiceChanged += OnServiceChanged;
            serviceBrowserEmbedded.StartBrowse(serviceTypeEmbedded);
        }

        /// <summary>
        /// Callback for when a device is changed.
        /// </summary>
        private void OnServiceChanged(object sender, ServiceAnnouncementEventArgs e)
        {
            //TODO
        }

        /// <summary>
        /// Callback for when a device is removed.
        /// </summary>
        private void OnServiceRemoved(object sender, ServiceAnnouncementEventArgs e)
        {
            //TODO
        }

        /// <summary>
        /// Callback for when a device is added.
        /// </summary>
        private void OnServiceAdded(object sender, ServiceAnnouncementEventArgs e)
        {
            if (e == null || e.Announcement == null || e.Announcement.Addresses == null || e.Announcement.Txt == null
                || discoveredDevices == null)
                return;

            var discoveredDevice = new DiscoveredDevice
            {
                IPAddress = e.Announcement.Addresses[0].ToString(),
                Protocol = e.Announcement.Type,
                Port = e.Announcement.Port,
                Name = e.Announcement.Txt.Where(x => x.ToString().StartsWith("fn=")).FirstOrDefault()?.Replace("fn=", ""),
                Headers = JsonConvert.SerializeObject(e.Announcement.Txt),
                Usn = e.Announcement.Hostname,
            };

            if (discoveredDevice.Name != null 
                && discoveredDevice.Usn != null 
                && discoveredDevice.Headers != null
                && (discoveredDevice.Protocol.IndexOf(serviceType) >= 0 
                    || discoveredDevice.Protocol.IndexOf(serviceTypeEmbedded) >= 0))
            {
                discoveredDevices.Add(discoveredDevice);
            }
        }

        /// <summary>
        /// Call the callback function for the discovered devices.
        /// </summary>
        private void OnAddDevice(object sender, ElapsedEventArgs e)
        {
            if (discoveredDevices == null)
                return;

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