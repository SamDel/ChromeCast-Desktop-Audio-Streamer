using System.Collections.Generic;

namespace ChromeCast.Desktop.AudioStreamer.Discover
{
    public class DiscoveredDevice
    {
        public static string GroupIdentifier = "\"md=Google Cast Group\"";

        public string Name { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public string Protocol { get; set; }
        public string Usn { get; set; }
        public string Headers { get; set; }
        public bool IsGroup {
            get
            {
                if (Headers != null && Headers.IndexOf(GroupIdentifier) >= 0)
                    return true;

                return false;
            }
        }
    }
}
