using System;
using Rssdp;

namespace ChromeCast.Desktop.AudioStreamer.Discover.Interfaces
{
    public interface IDiscoverServiceSSDP
    {
        void Discover(Action<DiscoveredSsdpDevice, SsdpDevice> onDiscovered, Action updateCounter);
    }
}