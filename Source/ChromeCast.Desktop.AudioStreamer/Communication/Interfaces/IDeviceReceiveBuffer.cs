using System;
using ChromeCast.Desktop.AudioStreamer.ProtocolBuffer;

namespace ChromeCast.Desktop.AudioStreamer.Communication.Interfaces
{
    public interface IDeviceReceiveBuffer
    {
        void OnReceive(byte[] data);
        void SetCallback(Action<CastMessage> onReceiveMessage);
    }
}