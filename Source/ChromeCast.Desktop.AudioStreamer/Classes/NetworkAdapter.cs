using System.Net;

namespace ChromeCast.Desktop.AudioStreamer.Classes
{
    public class NetworkAdapter
    {
        public IPAddress IPAddress { get; set; }
        public bool IsEthernet { get; set; }
    }
}
