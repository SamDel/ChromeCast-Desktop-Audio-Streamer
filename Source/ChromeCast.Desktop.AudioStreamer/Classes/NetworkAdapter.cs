using System.Net;

namespace ChromeCast.Desktop.AudioStreamer.Classes
{
    /// <summary>
    /// Represents a PC's network adapter.
    /// </summary>
    public class NetworkAdapter
    {
        public IPAddress IPAddress { get; set; }
        public bool IsEthernet { get; set; }
    }
}
