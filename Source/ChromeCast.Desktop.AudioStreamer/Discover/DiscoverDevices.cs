using System;
using System.Timers;
using Rssdp;
using ChromeCast.Desktop.AudioStreamer.Discover.Interfaces;

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
            discoverServiceSSDP.Discover(onDiscovered, UpdateCounter);

            timer = new Timer();
            timer.Interval = Interval;
            timer.Enabled = true;
            timer.Elapsed += new ElapsedEventHandler(OnDiscoverDevices);
            timer.Start();

            numberOfTries = 0;
            numberDiscovered = 0;
        }

        private void OnDiscoverDevices(object sender, ElapsedEventArgs e)
        {
            if (numberDiscovered == 0 && numberOfTries <= MaxNumberOfTries)
                discoverServiceSSDP.Discover(onDiscovered, UpdateCounter);
            else
                timer.Stop();

            numberOfTries++;
        }

        public void UpdateCounter()
        {
            numberDiscovered++;
        }
    }
}