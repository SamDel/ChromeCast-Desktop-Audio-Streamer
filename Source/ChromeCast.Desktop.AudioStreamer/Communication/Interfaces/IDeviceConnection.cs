using System;
using ChromeCast.Desktop.AudioStreamer.ProtocolBuffer;

namespace ChromeCast.Desktop.AudioStreamer.Communication.Interfaces
{
    public interface IDeviceConnection
    {
        void SendMessage(byte[] byteMessage);
        bool IsConnected();
        void SetCallback(Func<string> getHost, Func<int> getPort, Action<DeviceState, string> setDeviceState, Action<CastMessage> onReceiveMessage, Action<Action> startTask);
        void Dispose();
        void ReConnect();
    }
}