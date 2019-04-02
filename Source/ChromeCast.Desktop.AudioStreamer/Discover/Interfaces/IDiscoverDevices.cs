using System;

namespace ChromeCast.Desktop.AudioStreamer.Discover.Interfaces
{
    public interface IDiscoverDevices
    {
        void Discover(Action<DiscoveredDevice> onDiscovered);
        void Dispose();
    }
}