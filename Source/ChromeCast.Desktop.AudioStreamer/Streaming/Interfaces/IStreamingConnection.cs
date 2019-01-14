using System.Net.Sockets;
using ChromeCast.Desktop.AudioStreamer.Classes;
using NAudio.Wave;

namespace ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces
{
    public interface IStreamingConnection
    {
        void SendData(byte[] dataToSend, WaveFormat format, int reduceLagThreshold, SupportedStreamFormat streamFormat);
        void SendStartStreamingResponse();
        bool IsConnected();
        void SetSocket(Socket socket);
        string GetRemoteEndPoint();
    }
}