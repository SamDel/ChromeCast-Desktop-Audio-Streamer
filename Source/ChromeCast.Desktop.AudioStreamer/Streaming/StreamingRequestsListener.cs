using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces;
using Microsoft.Win32;

namespace ChromeCast.Desktop.AudioStreamer.Streaming
{
    public class StateObject
    {
        public Socket workSocket = null;
        public const int bufferSize = 2048;
        public byte[] buffer;
        public StringBuilder receiveBuffer = new StringBuilder();
    }

    public class StreamingRequestsListener : IStreamingRequestsListener
    {
        public ManualResetEvent allDone = new ManualResetEvent(false);
        private Action<string, int> onListenCallback;
        private Action<Socket, string> onConnectCallback;
        private Socket listener;

        public void StartListening(Action<string, int> onListenCallbackIn, Action<Socket, string> onConnectCallbackIn)
        {
            onListenCallback = onListenCallbackIn;
            onConnectCallback = onConnectCallbackIn;
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = GetIp4Address(ipHostInfo);
            var localEndPoint = new IPEndPoint(ipAddress, 0);
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);
                onListenCallback?.Invoke(((IPEndPoint)listener.LocalEndPoint).Address.ToString(), ((IPEndPoint)listener.LocalEndPoint).Port);

                while (true)
                {
                    allDone.Reset();
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                    allDone.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void StopListening()
        {
            try
            {
                listener.Close();
            }
            catch (Exception)
            {
            }
        }

        private void AcceptCallback(IAsyncResult asyncResult)
        {
            allDone.Set();

            var listener = (Socket)asyncResult.AsyncState;
            try
            {
                var handlerSocket = listener.EndAccept(asyncResult);
                var state = new StateObject { workSocket = handlerSocket };

                state.buffer = new byte[StateObject.bufferSize];
                handlerSocket.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private void ReadCallback(IAsyncResult asyncResult)
        {
            var state = (StateObject)asyncResult.AsyncState;
            var handlerSocket = state.workSocket;

            var bytesRead = handlerSocket.EndReceive(asyncResult);
            if (bytesRead > 0)
            {
                state.receiveBuffer.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                if (state.receiveBuffer.ToString().IndexOf("\r\n\r\n") >= 0)
                {
                    onConnectCallback?.Invoke(handlerSocket, state.receiveBuffer.ToString());
                }
                else
                {
                    // Not all data received. Get more.  
                    handlerSocket.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private IPAddress GetIp4Address(IPHostEntry ipHostInfo)
        {
            var addressesWireless = GetIp4ddresses();
            var ipAddress = ipHostInfo.AddressList[0];
            foreach (var address in ipHostInfo.AddressList)
            {
                if (address.AddressFamily.Equals(AddressFamily.InterNetwork) && addressesWireless.Contains(address))
                    ipAddress = address;
            }

            return ipAddress;
        }

        private List<IPAddress> GetIp4ddresses()
        {
            var ipAddresses = new List<IPAddress>();

            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var networkInterface in networkInterfaces)
            {
                foreach (var ip in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    var address = ip.Address;
                    if (address.AddressFamily == AddressFamily.InterNetwork
                        && networkInterface.NetworkInterfaceType.Equals(NetworkInterfaceType.Wireless80211)
                        && (address.ToString().StartsWith("192.168.")
                            || address.ToString().StartsWith("10.")
                            || address.ToString().StartsWith("172.")))
                    {
                        ipAddresses.Add(address);
                    }
                }
            }

            return ipAddresses;
        }

        private static bool IsNetworkCard(NetworkInterface adapter)
        {
            var fRegistryKey = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\" + adapter.Id + "\\Connection";
            var key = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
            if (key != null)
            {
                var fPnpInstanceID = key.GetValue("PnpInstanceID", "").ToString();
                var fMediaSubType = Convert.ToInt32(key.GetValue("MediaSubType", 0));
                if (fPnpInstanceID?.Length > 3 && fPnpInstanceID?.Substring(0, 3) == "PCI")
                    return true;
                else if (fMediaSubType == 2)
                    return true;
            }

            return false;
        }
    }
}