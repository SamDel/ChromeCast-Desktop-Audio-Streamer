using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ChromeCast.Desktop.AudioStreamer.Classes;
using ChromeCast.Desktop.AudioStreamer.Discover;
using ChromeCast.Desktop.AudioStreamer.Streaming;
using NAudio.Wave;

namespace ChromeCast.Desktop.AudioStreamer.Application.Interfaces
{
    public interface IApplicationLogic
    {
        void Initialize();
        void SetLagThreshold(int lagThreshold);
        void OnAddDevice(IDevice device);
        void OnRecordingDataAvailable(byte[] dataToSend, WaveFormat waveFormat);
        void OnStreamingRequestConnect(Socket handlerSocket, string httpRequest);
        void SetDependencies(MainForm mainForm);
        void CloseApplication();
        void OnSetAutoRestart(bool autoRestart);
        bool GetAutoRestart();
        void ChangeIPAddressUsed(IPAddress ipAddress);
        void ScanForDevices();
        void ResetSettings();
        void SetStreamFormat(SupportedStreamFormat format);
        void SetCulture(string culture);
        string GetStreamingUrl();
        void SetFilterDevices(FilterDevicesEnum value);
        bool WasPlaying(DiscoveredDevice discoveredDevice);
        void ClearMp3Buffer();
        void StartTask(Action action, CancellationTokenSource cancellationTokenSource = null);
        void SaveSettings();
        void SetRecordingDevice(RecordingDevice recordingDevice);
    }
}