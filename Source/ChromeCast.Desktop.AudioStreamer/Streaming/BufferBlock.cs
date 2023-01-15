using System;

namespace ChromeCast.Desktop.AudioStreamer.Streaming
{
    public class BufferBlock
    {
        public byte[] Data;
        public int Used;
        private int SpaceLeft { 
            get 
            {
                return Data.Length - Used;
            } 
        }

        /// <summary>
        /// Add bytes to the data buffer.
        /// </summary>
        /// <param name="dataArray">byte array with the bytes to add</param>
        /// <param name="numberOfBytes">number of bytes in the byte array</param>
        internal void Add(byte[] dataArray, int numberOfBytes)
        {
            if (numberOfBytes <= SpaceLeft)
            {
                Array.Copy(dataArray, 0, Data, Used, numberOfBytes);
                Used += numberOfBytes;
            }
        }
    }
}