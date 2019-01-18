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
        void VolumeSet(float level);
        void VolumeUp();
        void VolumeDown();
        void VolumeMute();
        void SetCallback(Action<DeviceState, string> setDeviceState, Action<Volume> onVolumeUpdate,  
            Func<string> getHost);
        bool Stop();
        void Close();
        void Dispose();
        DeviceState GetDeviceState();
    }
}