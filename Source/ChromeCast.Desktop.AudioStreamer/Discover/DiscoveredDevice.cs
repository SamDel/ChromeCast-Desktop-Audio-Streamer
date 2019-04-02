using ChromeCast.Desktop.AudioStreamer.Application;
using ChromeCast.Desktop.AudioStreamer.Communication;
using System.Xml.Serialization;

namespace ChromeCast.Desktop.AudioStreamer.Discover
{
    public class DiscoveredDevice
    {
        private const string GroupIdentifier = "\"md=Google Cast Group\"";

        public string Name { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public string Protocol { get; set; }
        public string Usn { get; set; }
        public string Headers { get; set; }
        public bool AddedByDeviceInfo { get; set; }
        [XmlIgnore]
        public DeviceEureka Eureka { get; set; }
        [XmlIgnore]
        public Group Group { get; set; }
        public DeviceState DeviceState { get; set; }
        public bool IsGroup {
            get
            {
                if (Headers != null && Headers.IndexOf(GroupIdentifier) >= 0)
                    return true;

                return false;
            }
            set
            {
                if (value)
                    Headers = GroupIdentifier;
            }
        }
    }
}
