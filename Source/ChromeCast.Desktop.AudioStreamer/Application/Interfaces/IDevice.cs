using System;
using System.Net.Sockets;
using System.Windows.Forms;
using NAudio.Wave;
using ChromeCast.Desktop.AudioStreamer.UserControls;
using ChromeCast.Desktop.AudioStreamer.Communication;
using ChromeCast.Desktop.AudioStreamer.Communication.Classes;
using ChromeCast.Desktop.AudioStreamer.ProtocolBuffer;
using ChromeCast.Desktop.AudioStreamer.Classes;
using ChromeCast.Desktop.AudioStreamer.Discover;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public interface IDevice
    {
        void SetDeviceState(DeviceState disposed, string text = null);
        void Initialize(DiscoveredDevice discoveredDevice);
        bool AddStreamingConnection(string remoteAddress, Socket socket);
        void OnGetStatus();
        void OnRecordingDataAvailable(byte[] dataToSend, WaveFormat format, int reduceLagThreshold, SupportedStreamFormat streamFormat);
        void OnClickPlayPause(object sender, EventArgs e);
        string GetUsn();
        string GetHost();
        string GetFriendlyName();
        DeviceState GetDeviceState();
        void SetDeviceControl(DeviceControl deviceControl);
        MenuItem GetMenuItem();
        void SetMenuItem(MenuItem menuItem);
        void VolumeUp();
        void VolumeDown();
        void VolumeMute();
        void VolumeSet(float level);
        void Stop();
        void Load();
        void OnReceiveMessage(CastMessage castMessage);
        DeviceControl GetDeviceControl();
        void SetDeviceName(string name);
        ushort GetPort();
        DiscoveredDevice GetDiscoveredDevice();
        void SendSilence();
    }
}