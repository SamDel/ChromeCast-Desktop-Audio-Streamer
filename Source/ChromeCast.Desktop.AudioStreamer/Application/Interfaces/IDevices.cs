using System;
using System.Net.Sockets;
using NAudio.Wave;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;
using System.Collections.Generic;
using ChromeCast.Desktop.AudioStreamer.Classes;
using ChromeCast.Desktop.AudioStreamer.Discover;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public interface IDevices
    {
        void AddStreamingConnection(Socket socket, string httpRequest);
        void OnGetStatus();
        void OnRecordingDataAvailable(byte[] dataToSend, WaveFormat format, int reduceLagThreshold, SupportedStreamFormat streamFormat);
        void OnDeviceAvailable(DiscoveredDevice discoveredDevice);
        void VolumeUp();
        void VolumeDown();
        void VolumeMute();
        void Start();
        bool Stop();
        void SetAutoStart(bool autoStart);
        void SetCallback(Action<Device> onAddDeviceCallbackIn);
        int Count();
        void Sync();
        void Dispose();
        void SetDependencies(MainForm mainFormIn, IApplicationLogic applicationLogicIn);
        List<DiscoveredDevice> GetHosts();
    }
}