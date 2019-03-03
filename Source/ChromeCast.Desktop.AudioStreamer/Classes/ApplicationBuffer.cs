using ChromeCast.Desktop.AudioStreamer.Application;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChromeCast.Desktop.AudioStreamer.Classes
{
    public class ApplicationBuffer
    {
        private readonly List<ApplicationBufferItem> applicationBuffer = new List<ApplicationBufferItem>();
        private WaveFormat waveFormat = new WaveFormat();
        private int reduceLagThreshold;
        private SupportedStreamFormat streamFormatSelected;
        private bool startBufferSend;
        private const double BufferSizeInBytesDefault = 310000;
        private double BufferSizeInBytes = BufferSizeInBytesDefault;
        private int ExtraBufferInSeconds = 0;

        /// <summary>
        /// Send the startup buffer, a buffer containing the past x seconds.
        /// </summary>
        /// <param name="device"></param>
        public void SendStartupBuffer(IDevice device, SupportedStreamFormat streamFormatIn)
        {
            if (device == null)
                return;

            streamFormatSelected = streamFormatIn;
            SetBufferSize();

            byte[] bufferAlreadySent;
            do
            {
                bufferAlreadySent = GetBufferAlreadySent();
                if (bufferAlreadySent.Length < BufferSizeInBytes)
                    Task.Delay(1000).Wait();
            } while (bufferAlreadySent.Length < BufferSizeInBytes);
            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] SendStartupBuffer: {bufferAlreadySent.Length}");
            device.OnRecordingDataAvailable(bufferAlreadySent, waveFormat, reduceLagThreshold, streamFormatSelected);
            startBufferSend = true;
        }

        /// <summary>
        /// Return if the satrt buffer is send.
        /// </summary>
        public bool IsStartBufferSend()
        {
            return startBufferSend;
        }

        /// <summary>
        /// Set the extra buffer value.
        /// </summary>
        /// <param name="extraBufferInSecondsIn"></param>
        public void SetExtraBufferInSeconds(int extraBufferInSecondsIn)
        {
            ExtraBufferInSeconds = extraBufferInSecondsIn;
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
            SetBufferSize();
        }

        private void SetBufferSize()
        {
            switch (streamFormatSelected)
            {
                case SupportedStreamFormat.Wav:
                    BufferSizeInBytes = ExtraBufferInSeconds * 5513000 + BufferSizeInBytesDefault;
                    break;
                case SupportedStreamFormat.Mp3_320:
                    BufferSizeInBytes = ExtraBufferInSeconds * 40000 + BufferSizeInBytesDefault;
                    break;
                case SupportedStreamFormat.Mp3_128:
                    BufferSizeInBytes = ExtraBufferInSeconds * 16000 + BufferSizeInBytesDefault;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Clear the buffer.
        /// </summary>
        public void ClearBuffer()
        {
            if (applicationBuffer == null)
                return;

            lock (applicationBuffer)
            {
                applicationBuffer.Clear();
            }
            startBufferSend = false;
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
        private byte[] GetBufferAlreadySent()
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

            return buffer.ToArray();
        }
    }
}
