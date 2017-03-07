using Rssdp;
using System;

namespace ChromeCast.Desktop.AudioStreamer.Discover.Interfaces
{
    public interface IDiscoverServiceSSDP
    {
        void Discover(Action<DiscoveredSsdpDevice, SsdpDevice> onDiscovered);
    }
}