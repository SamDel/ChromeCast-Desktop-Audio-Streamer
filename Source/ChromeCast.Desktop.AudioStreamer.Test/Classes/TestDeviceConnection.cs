using System;
using System.Collections.Generic;
using ChromeCast.Desktop.AudioStreamer.Communication;
using ChromeCast.Desktop.AudioStreamer.Communication.Interfaces;
using ChromeCast.Desktop.AudioStreamer.ProtocolBuffer;

namespace ChromeCast.Desktop.AudioStreamer.Test.Classes
{
    public class TestDeviceConnection : IDeviceConnection
    {
        private Func<string> getHost;
        private Action<DeviceState, string> setDeviceState;
        private Action<CastMessage> onReceiveMessage;
        public IList<CastMessage> messagesSend = new List<CastMessage>();

        public bool IsConnected()
        {
            return true;
        }

        public void SendMessage(byte[] byteMessage)
        {
            var castMessage = CastMessage.ParseFrom(SubArray(byteMessage, 4, byteMessage.Length - 4));
            messagesSend.Add(castMessage);
        }

        public void SetCallback(Func<string> getHostIn, Action<DeviceState, string> setDeviceStateIn, Action<CastMessage> onReceiveMessageIn)
        {
            getHost = getHostIn;
            setDeviceState = setDeviceStateIn;
            onReceiveMessage = onReceiveMessageIn;
        }

        private T[] SubArray<T>(T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
