using System.Net;
using System.Net.Sockets;
using ChromeCast.Desktop.AudioStreamer.Classes;
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
        void RecordingDeviceChanged();
        void OnSetAutoRestart(bool autoRestart);
        bool GetAutoRestart();
        void ChangeIPAddressUsed(IPAddress ipAddress);
        void ScanForDevices();
        void ResetSettings();
        void SetStreamFormat(SupportedStreamFormat format);
        void SetCulture(string culture);
        string GetStreamingUrl();
    }
}