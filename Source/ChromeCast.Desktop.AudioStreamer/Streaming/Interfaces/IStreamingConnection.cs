using System.Net.Sockets;
using NAudio.Wave;

namespace ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces
{
    public interface IStreamingConnection
    {
        void SendData(byte[] dataToSend, WaveFormat format, int reduceLagThreshold);
        void SendStartStreamingResponse();
        bool IsMaxWavSizeReached(int length);
        bool IsConnected();
        void SetSocket(Socket socket);
        string GetRemoteEndPoint();
    }
}