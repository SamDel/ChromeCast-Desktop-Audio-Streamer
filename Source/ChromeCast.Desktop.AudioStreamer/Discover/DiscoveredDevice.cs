using System.Collections.Generic;

namespace ChromeCast.Desktop.AudioStreamer.Discover
{
    public class DiscoveredDevice
    {
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public ushort Port { get; set; }
        public string Protocol { get; set; }
        public string Usn { get; set; }
        public string Headers { get; set; }
    }
}
