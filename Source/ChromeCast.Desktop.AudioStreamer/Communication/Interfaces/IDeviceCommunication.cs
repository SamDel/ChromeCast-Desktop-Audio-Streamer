using System;
using ChromeCast.Desktop.AudioStreamer.Application;
using ChromeCast.Desktop.AudioStreamer.Communication.Classes;
using ChromeCast.Desktop.AudioStreamer.ProtocolBuffer;

namespace ChromeCast.Desktop.AudioStreamer.Communication.Interfaces
{
    public interface IDeviceCommunication
    {
        void Connect(string sourceId = null, string destinationId = null);
        void Launch();
        void LaunchAndLoadMedia();
        void LoadMedia();
        void PauseMedia();
        void Pong();
        void GetStatus();
        string GetStatusText();
        void OnReceiveMessage(CastMessage castMessage);
        void VolumeSet(Volume volumeSetting);
        void VolumeMute(bool muted);
        void SetCallback(IDevice device, Action<byte[]> sendMessage, Func<bool> isDeviceConnected);
        void Disconnect();
        void Stop(bool changeUserMode = false);
        void OnPlayStop_Click();
        void OnStop_Click();
        void ResumePlaying();
        void Dispose();
    }
}