using System;
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
        void OnReceiveMessage(CastMessage castMessage);
        void VolumeSet(Volume volumeSetting);
        void VolumeMute(bool muted);
        void SetCallback(Action<DeviceState, string> setDeviceState, 
            Action<Volume> onVolumeUpdate, 
            Action<byte[]> sendMessage, 
            Func<DeviceState> getDeviceState, 
            Func<bool> isConnected, 
            Func<bool> isDeviceConnected, 
            Func<string> getHost, 
            Func<ushort> getPort);
        void Stop();
        void OnPlayPause_Click();
        void OnStop_Click();
    }
}