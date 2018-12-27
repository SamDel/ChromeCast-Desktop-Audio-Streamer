using System;
using System.Net;
using System.Net.Sockets;

namespace ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces
{
    public interface IStreamingRequestsListener
    {
        void StartListening(IPAddress ipAddress, Action<string, int> onListenCallbackIn, Action<Socket, string> onConnectCallbackIn);
        void StopListening();
    }
}