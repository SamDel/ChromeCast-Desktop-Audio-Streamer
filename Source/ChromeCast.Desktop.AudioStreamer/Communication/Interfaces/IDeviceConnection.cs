using System;
using ChromeCast.Desktop.AudioStreamer.ProtocolBuffer;

namespace ChromeCast.Desktop.AudioStreamer.Communication.Interfaces
{
    public interface IDeviceConnection
    {
        void SendMessage(byte[] byteMessage);
        bool IsConnected();
        void SetCallback(Func<string> getHost, Action<DeviceState, string> setDeviceState, Action<CastMessage> onReceiveMessage);
    }
}