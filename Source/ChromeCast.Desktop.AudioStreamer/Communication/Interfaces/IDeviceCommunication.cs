using System;
using ChromeCast.Desktop.AudioStreamer.Communication.Classes;
using ChromeCast.Desktop.AudioStreamer.ProtocolBuffer;

namespace ChromeCast.Desktop.AudioStreamer.Communication.Interfaces
{
    public interface IDeviceCommunication
    {
        void LoadMedia();
        void PauseMedia();
        void GetMediaStatus();
        void OnPlayPause_Click();
        void OnReceiveMessage(CastMessage castMessage);
        void VolumeSet(float level);
        void VolumeMute(bool muted);
        void SetCallback(Action<DeviceState, string> setDeviceState, Action<Volume> onVolumeUpdate,  
            Func<string> getHost);
        bool Stop();
        DeviceState GetDeviceState();
    }
}