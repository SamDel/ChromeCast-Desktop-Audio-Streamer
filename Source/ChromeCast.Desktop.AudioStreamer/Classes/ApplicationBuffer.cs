using ChromeCast.Desktop.AudioStreamer.Application;
using NAudio.Wave;
using System.Collections.Generic;
using System.Linq;

namespace ChromeCast.Desktop.AudioStreamer.Classes
{
    public class ApplicationBuffer
    {
        private readonly List<ApplicationBufferItem> applicationBuffer = new List<ApplicationBufferItem>();
        private WaveFormat waveFormat = new WaveFormat();
        private int reduceLagThreshold;
        private SupportedStreamFormat streamFormatSelected;
        private const int BufferSizeInBytes = 310000;

        public void SendStartupBuffer(IDevice device)
        {
            var bufferAlreadySent = GetBufferAlreadySent();
            device.OnRecordingDataAvailable(bufferAlreadySent, waveFormat, reduceLagThreshold, streamFormatSelected);
        }

        /// <summary>
        /// Add bytes to the application buffer.
        /// </summary>
        public void AddToBuffer(byte[] dataToSend, WaveFormat formatIn, int reduceLagThresholdIn, SupportedStreamFormat streamFormatIn)
        {
            waveFormat = formatIn;
            reduceLagThreshold = reduceLagThresholdIn;
            streamFormatSelected = streamFormatIn;
            KeepABuffer(dataToSend);
        }

        /// <summary>
        /// Clear the buffer.
        /// </summary>
        public void ClearBuffer()
        {
            lock (applicationBuffer)
            {
                applicationBuffer.Clear();
            }
        }

        /// <summary>
        /// Keep an application buffer.
        /// </summary>
        /// <param name="dataToSend"></param>
        private void KeepABuffer(byte[] dataToSend)
        {
            if (applicationBuffer == null)
                return;

            lock (applicationBuffer)
            {
                applicationBuffer.Insert(0, new ApplicationBufferItem { Data = dataToSend });

                while (applicationBuffer.Sum(x => x.Data.Length) - applicationBuffer.Last().Data.Length > BufferSizeInBytes)
                {
                    applicationBuffer.RemoveAt(applicationBuffer.Count - 1);
                }
            }
        }

        /// <summary>
        /// Get the data in the application buffer that's already send to the devices.
        /// </summary>
        public byte[] GetBufferAlreadySent()
        {
            if (applicationBuffer == null)
                return new byte[0];

            IEnumerable<byte> buffer = new List<byte>();
            lock (applicationBuffer)
            {
                for (int i = applicationBuffer.Count - 1; i >= 0; i--)
                {
                    buffer = buffer.Concat(applicationBuffer[i].Data);
                }
            }

            // Hack to start mp3 streams faster: Send a 7.8 seconds buffer of 320 kbps silence.
            if (streamFormatSelected != SupportedStreamFormat.Wav && buffer.ToArray().Length < BufferSizeInBytes)
                buffer = Properties.Resources.silence.Concat(buffer);

            return buffer.ToArray();
        }
    }
}
