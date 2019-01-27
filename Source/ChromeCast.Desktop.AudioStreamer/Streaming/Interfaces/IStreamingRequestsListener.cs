using System;
using System.Net;
using System.Net.Sockets;

namespace ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces
{
    public interface IStreamingRequestsListener
    {
        void StartListening(IPAddress ipAddress, Action<Socket, string> onConnectCallbackIn);
        void StopListening();
        string GetStreamimgUrl();
    }
}