using System;
using System.Net.Sockets;

namespace ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces
{
    public interface IStreamingRequestsListener
    {
        void StartListening(Action<string, int> onListenCallbackIn, Action<Socket, string> onConnectCallbackIn);
        void StopListening();
    }
}