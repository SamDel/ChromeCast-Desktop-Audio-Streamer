using System;
using System.IO;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;
using NAudio.Lame;
using NAudio.Wave;

namespace ChromeCast.Desktop.AudioStreamer.Classes
{
    /// <summary>
    /// MP3 encode a WAV stream.
    /// </summary>
    public class Mp3Stream
    {
        private MemoryStream Output { get; set; }
        private LameMP3FileWriter Writer { get; set; }
        public ILogger logger { get; set; }

        public Mp3Stream(ILogger loggerIn)
        {
            logger = loggerIn;
        }

        /// <summary>
        /// Setup MP3 encoding with the selected WAV and stream formats.
        /// </summary>
        /// <param name="format">the WAV input format</param>
        /// <param name="formatSelected">the mp3 output format</param>
        public Mp3Stream(WaveFormat format, SupportedStreamFormat formatSelected)
        {
            Output = new MemoryStream();
            var bitRate = 128;
            if (formatSelected.Equals(SupportedStreamFormat.Mp3_320))
                bitRate = 320;

            Writer = new LameMP3FileWriter(Output, format, bitRate);
        }

        /// <summary>
        /// Add WAV data that should be encoded. 
        /// </summary>
        /// <param name="buffer"></param>
        public void Encode(byte[] buffer)
        {
            if (Writer == null || buffer == null)
                return;

            try
            {
                Writer.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                logger.Log($"ex : {ex.Message}");
            }
        }

        /// <summary>
        /// Read the data that's encoded in MP3 format.
        /// </summary>
        /// <returns></returns>
        public byte[] Read()
        {
            if (Output == null)
                return new byte[0];

            var byteArray = Output.ToArray();
            Output.SetLength(0);
            return byteArray;
        }
    }
}
