using System;
using System.Windows.Forms;
using Rssdp;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Discover.Interfaces;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class DiscoverDevices : IDiscoverDevices
    {
        private IDiscoverServiceSSDP discoverServiceSSDP;
        private Action<DiscoveredSsdpDevice, SsdpDevice> onDiscovered;
        private Timer timer;
        private int numberOfTries;
        private int numberDiscovered;

        public DiscoverDevices(IDiscoverServiceSSDP discoverServiceSSDPIn)
        {
            discoverServiceSSDP = discoverServiceSSDPIn;

            timer = new Timer();
            timer.Interval = 3000;
            timer.Tick += new EventHandler(OnDiscoverDevices);
            timer.Start();

            numberOfTries = 0;
            numberDiscovered = 0;
        }

        public void Discover(Action<DiscoveredSsdpDevice, SsdpDevice> onDiscoveredIn)
        {
            onDiscovered = onDiscoveredIn;
            discoverServiceSSDP.Discover(onDiscovered);
        }

        private void OnDiscoverDevices(object sender, EventArgs e)
        {
            if (numberDiscovered == 0 && numberOfTries <= 10)
                discoverServiceSSDP.Discover(onDiscovered);
            else
                timer.Stop();

            numberDiscovered++;
            numberOfTries++;
        }
    }
}