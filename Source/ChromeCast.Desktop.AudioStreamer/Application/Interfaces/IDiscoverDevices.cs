using System;
using Rssdp;

namespace ChromeCast.Desktop.AudioStreamer.Application.Interfaces
{
    public interface IDiscoverDevices
    {
        void Discover(Action<DiscoveredSsdpDevice, SsdpDevice> onDiscovered);
    }
}