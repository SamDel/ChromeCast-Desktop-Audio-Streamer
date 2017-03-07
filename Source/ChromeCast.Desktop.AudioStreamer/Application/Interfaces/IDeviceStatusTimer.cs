using System;

namespace ChromeCast.Desktop.AudioStreamer.Application.Interfaces
{
    public interface IDeviceStatusTimer
    {
        void StartPollingDevice(Action onGetStatus);
    }
}