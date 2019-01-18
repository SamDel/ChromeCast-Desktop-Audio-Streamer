using System;
using System.Net.Sockets;
using System.Windows.Forms;
using Rssdp;
using NAudio.Wave;
using ChromeCast.Desktop.AudioStreamer.UserControls;
using ChromeCast.Desktop.AudioStreamer.Communication;
using ChromeCast.Desktop.AudioStreamer.Communication.Classes;
using ChromeCast.Desktop.AudioStreamer.ProtocolBuffer;
using ChromeCast.Desktop.AudioStreamer.Classes;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public interface IDevice
    {
        void SetDeviceState(DeviceState disposed, string text = null);
        void SetDiscoveredDevices(DiscoveredSsdpDevice device, SsdpDevice fullDevice);
        bool AddStreamingConnection(string remoteAddress, Socket socket);
        void OnGetStatus();
        void OnRecordingDataAvailable(byte[] dataToSend, WaveFormat format, int reduceLagThreshold, SupportedStreamFormat streamFormat);
        void OnClickDeviceButton(object sender, EventArgs e);
        string GetUsn();
        string GetHost();
        string GetFriendlyName();
        void SetDeviceControl(DeviceControl deviceControl);
        void SetMenuItem(MenuItem menuItem);
        void OnVolumeUpdate(Volume volume);
        void VolumeUp();
        void VolumeDown();
        void VolumeMute();
        void VolumeSet(float level);
        bool Stop();
        void Start();
        void SetDeviceName(string name);
    }
}