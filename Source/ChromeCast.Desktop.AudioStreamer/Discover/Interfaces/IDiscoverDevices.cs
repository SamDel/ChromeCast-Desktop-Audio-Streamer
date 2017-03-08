using System;
using Rssdp;

namespace ChromeCast.Desktop.AudioStreamer.Discover.Interfaces
{
    public interface IDiscoverDevices
    {
        void Discover(Action<DiscoveredSsdpDevice, SsdpDevice> onDiscovered);
    }
}