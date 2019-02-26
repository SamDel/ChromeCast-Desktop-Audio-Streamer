using System.Net.Sockets;
using ChromeCast.Desktop.AudioStreamer.Application;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Classes;
using NAudio.Wave;

namespace ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces
{
    public interface IStreamingConnection
    {
        void SendData(byte[] dataToSend, WaveFormat format, int reduceLagThreshold, SupportedStreamFormat streamFormat);
        void SendStartStreamingResponse();
        bool IsConnected();
        void SetDependencies(Socket socketIn, IDevice deviceIn, ILogger loggerIn);
        string GetRemoteEndPoint();
        void Dispose();
    }
}