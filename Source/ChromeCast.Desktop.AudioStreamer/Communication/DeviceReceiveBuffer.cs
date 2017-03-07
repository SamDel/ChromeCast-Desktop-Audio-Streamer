using System;
using System.Linq;
using ChromeCast.Desktop.AudioStreamer.ProtocolBuffer;
using ChromeCast.Desktop.AudioStreamer.Communication.Interfaces;

namespace ChromeCast.Desktop.AudioStreamer.Communication
{
    public class DeviceReceiveBuffer : IDeviceReceiveBuffer
    {
        private Action<CastMessage> onReceiveMessage;

        public void OnReceive(byte[] data)
        {
            ParseMessages(data);
        }

        private void ParseMessages(byte[] serverMessage)
        {
            int offset = 0;

            while (serverMessage.Length - offset >= 4)
            {
                var messageSize = BitConverter.ToInt32(serverMessage.Skip(offset).Take(4).Reverse().ToArray(), 0);
                if (messageSize == 0)
                    break;

                if (serverMessage.Length >= 4 + messageSize)
                {
                    var message = SubArray(serverMessage, 4, messageSize);
                    ProcessMessage(message);

                    offset = offset + 4 + messageSize;
                }
            }
        }

        private void ProcessMessage(byte[] message)
        {
            var castMessage = CastMessage.ParseFrom(message);
            onReceiveMessage?.Invoke(castMessage);
        }

        private T[] SubArray<T>(T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public void SetCallback(Action<CastMessage> onReceiveMessageIn)
        {
            onReceiveMessage = onReceiveMessageIn;
        }
    }
}
