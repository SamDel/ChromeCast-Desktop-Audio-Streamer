using System;
using System.Net.Sockets;
using Rssdp;
using NAudio.Wave;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public interface IDevices
    {
        void AddStreamingConnection(Socket socket, string httpRequest);
        void OnGetStatus();
        void OnRecordingDataAvailable(byte[] dataToSend, WaveFormat format, int reduceLagThreshold);
        void OnDeviceAvailable(DiscoveredSsdpDevice discoveredSsdpDevice, SsdpDevice ssdpRootDevice);
        void VolumeUp();
        void VolumeDown();
        void VolumeMute();
        void SetAutoStart(bool autoStart);
        void SetCallback(Action<Device> onAddDeviceCallbackIn);
        void Dispose();
    }
}