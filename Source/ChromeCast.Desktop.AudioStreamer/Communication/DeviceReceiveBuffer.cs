using System;
using System.Linq;
using ChromeCast.Desktop.AudioStreamer.ProtocolBuffer;
using ChromeCast.Desktop.AudioStreamer.Communication.Interfaces;

namespace ChromeCast.Desktop.AudioStreamer.Communication
{
    /// <summary>
    /// Receive buffer used by the socket connection used for control messages.
    /// If a complete message is in the buffer then a call to the callback function is made.
    /// </summary>
    public class DeviceReceiveBuffer : IDeviceReceiveBuffer
    {
        private Action<CastMessage> onReceiveMessage;

        /// <summary>
        /// Received data from a device.
        /// </summary>
        /// <param name="data">the received data</param>
        public void OnReceive(byte[] data)
        {
            if (data == null)
                return;

            ParseMessages(data);
        }

        /// <summary>
        /// Parse the receive buffer. 
        /// If a message is complete call the method to process the message.
        /// </summary>
        /// <param name="serverMessage">data received from a device</param>
        private void ParseMessages(byte[] serverMessage)
        {
            if (serverMessage == null)
                return;

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

        /// <summary>
        /// Process a message.
        /// </summary>
        /// <param name="message">the message</param>
        private void ProcessMessage(byte[] message)
        {
            if (message == null)
                return;

            try
            {
                var castMessage = CastMessage.ParseFrom(message);
                onReceiveMessage?.Invoke(castMessage);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Get a part of the buffer.
        /// </summary>
        private T[] SubArray<T>(T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        /// <summary>
        /// Set the callback.
        /// </summary>
        public void SetCallback(Action<CastMessage> onReceiveMessageIn)
        {
            onReceiveMessage = onReceiveMessageIn;
        }
    }
}
