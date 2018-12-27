using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ChromeCast.Desktop.AudioStreamer.Classes
{
    public static class Network
    {
        public static List<NetworkAdapter> GetIp4ddresses()
        {
            var adapters = new List<NetworkAdapter>();

            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var networkInterface in networkInterfaces)
            {
                foreach (var ip in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    var address = ip.Address;
                    var props = networkInterface.GetIPProperties();
                    if (address.AddressFamily == AddressFamily.InterNetwork
                        && (address.ToString().StartsWith("192.168.")
                            || address.ToString().StartsWith("10.")
                            || address.ToString().StartsWith("172."))
                        && networkInterface.OperationalStatus != OperationalStatus.Down
                        && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback
                        && props.GatewayAddresses.Count > 0)
                    {
                        adapters.Add(
                            new NetworkAdapter {
                                IPAddress = address,
                                IsEthernet = networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet || 
                                                networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet3Megabit ||
                                                networkInterface.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet ||
                                                networkInterface.NetworkInterfaceType == NetworkInterfaceType.FastEthernetFx ||
                                                networkInterface.NetworkInterfaceType == NetworkInterfaceType.FastEthernetT
                            });
                    }
                }
            }

            return adapters;
        }

        public static IPAddress GetIp4Address()
        {
            var addressesInUse = GetIp4ddresses();
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList[0];
            var addressFound = false;
            foreach (var address in ipHostInfo.AddressList)
            {
                var adapter = addressesInUse.Where(x => x.IPAddress.ToString() == address.ToString()).FirstOrDefault();
                if (adapter != null && 
                    !address.IsIPv4MappedToIPv6 && 
                    !address.IsIPv6LinkLocal && 
                    !address.IsIPv6Multicast && 
                    !address.IsIPv6SiteLocal && 
                    !address.IsIPv6Teredo)
                {
                    if (!addressFound || adapter.IsEthernet)
                    {
                        addressFound = true;
                        ipAddress = address;
                    }
                }
            }
            return ipAddress;
        }
    }
}
