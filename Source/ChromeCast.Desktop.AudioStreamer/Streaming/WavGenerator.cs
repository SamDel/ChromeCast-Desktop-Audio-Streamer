using NAudio.Wave;
using System;
using System.IO;

namespace ChromeCast.Desktop.AudioStreamer.Streaming
{
    /// <summary>
    /// A class that plays generated wav content.
    /// </summary>
    public class WavGenerator
    {
        private WaveFormat waveFormat = new WaveFormat(44100, 2);
        private WaveOutEvent player;

        /// <summary>
        /// Play (looping) silence, to make sure there always something captured.
        /// </summary>
        public void PlaySilenceLoop(string deviceName, CSCore.WaveFormat deviceFormat)
        {
            waveFormat = new WaveFormat(deviceFormat.SampleRate, deviceFormat.Channels);
            Play(GenerateSilence(60), deviceName);
        }

        /// <summary>
        /// Stop playing.
        /// </summary>
        public void Stop()
        {
            if (player == null)
                return;

            player.Stop();
        }

        /// <summary>
        /// Play a loop stream.
        /// </summary>
        /// <param name="stream">the stream to play.</param>
        private void Play(LoopStream stream, string deviceName)
        {
            if (stream == null)
                return;

            try
            {
                player = new WaveOutEvent();
                if (!string.IsNullOrEmpty(deviceName))
                {
                    for (int i = -1; i < WaveOut.DeviceCount; i++)
                    {
                        var capabilities = WaveOut.GetCapabilities(i);
                        if (deviceName.StartsWith(capabilities.ProductName))
                            player.DeviceNumber = i;
                    }
                }
                player.Init(new WaveChannel32(stream, 0.0f, 0));
                player.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Generate a wav loop stream containing silence.
        /// </summary>
        /// <param name="nrSeconds">the length of the wav stream in seconds</param>
        /// <returns>a loop stream containing 'nrSeconds' of wav silence</returns>
        private LoopStream GenerateSilence(uint nrSeconds)
        {
            return CreateStream(GetSilence(nrSeconds));
        }

        /// <summary>
        /// Create a byte array with silence wav data.
        /// </summary>
        /// <param name="seconds">number of seconds of the wav data</param>
        /// <returns>'seconds' of wav data in a byte array</returns>
        public byte[] GetSilenceBytes(uint seconds)
        {
            var silence = GetSilence(seconds);
            var result = new byte[silence.AudioSamples.Length * sizeof(short)];
            Buffer.BlockCopy(silence.AudioSamples, 0, result, 0, result.Length);
            return result;
        }

        /// <summary>
        /// Get silence data.
        /// </summary>
        private WaveDataChunk GetSilence(uint nrSeconds)
        {
            // Fill the data array with silence samples.
            var data = new WaveDataChunk();
            var numSamples = waveFormat.SampleRate * waveFormat.Channels * nrSeconds;
            data.AudioSamples = new short[numSamples];
            for (uint i = 0; i < numSamples - 1; i++)
            {
                for (int channel = 0; channel < waveFormat.Channels; channel++)
                {
                    data.AudioSamples[i + channel] = 0;
                }
            }
            data.Length = (uint)(data.AudioSamples.Length * (waveFormat.BitsPerSample / 8));
            return data;
        }

        /// <summary>
        /// Generate a wav loop stream containing a 440Hz sine wave.
        /// </summary>
        /// <param name="nrSeconds">the length of the wav stream in seconds</param>
        /// <returns>a loop stream containing 'nrSeconds' of wav sine wave</returns>
        private LoopStream GenerateSineWave(uint nrSeconds)
        {
            // Fill the data array with sample data
            var data = new WaveDataChunk();
            var numSamples = waveFormat.SampleRate * waveFormat.Channels * nrSeconds;
            data.AudioSamples = new short[numSamples];
            var amplitude = 32760;  // Max amplitude for 16-bit audio
            var freq = 440.0f;   // Concert A: 440Hz
  
            // The "angle" used in the function, adjusted for the number of channels and sample rate.
            // This value is like the period of the wave.
            double t = (Math.PI * 2 * freq) / (waveFormat.SampleRate * waveFormat.Channels);
  
            for (uint i = 0; i < numSamples - 1; i++)
            {
                // Fill with a simple sine wave at max amplitude
                for (int channel = 0; channel<waveFormat.Channels; channel++)
                {
                    data.AudioSamples[i + channel] = Convert.ToInt16(amplitude * Math.Sin(t * i));
                }                        
            }
            data.Length = (uint)(data.AudioSamples.Length * (waveFormat.BitsPerSample / 8));

            return CreateStream(data);
        }

        /// <summary>
        /// Create a loop stream of the wav data.
        /// </summary>
        /// <param name="data">wav data</param>
        /// <returns>a loop stream containing the wav data</returns>
        private LoopStream CreateStream(WaveDataChunk data)
        {
            if (data == null)
                return null;

            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);

            // Write the wav header
            var header = new AudioHeader();
            writer.Write(header.GetRiffHeader(waveFormat, data.Length));

            // Write the data chunk
            foreach (short dataPoint in data.AudioSamples)
            {
                writer.Write(dataPoint);
            }

            writer.Seek(4, SeekOrigin.Begin);
            var streamsize = (uint)writer.BaseStream.Length;
            writer.Write(streamsize - 8);
            writer.Flush();
            stream.Position = 0;

            return new LoopStream(stream, waveFormat);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            player?.Stop();
            player?.Dispose();
        }
    }

    /// <summary>
    /// A wavestream that's returns the same stream in a loop.
    /// </summary>
    public class LoopStream : WaveStream
    {
        readonly MemoryStream memoryStream;
        readonly WaveFormat format;

        public LoopStream(WaveStream source)
        {
            throw new NotImplementedException();
        }

        public LoopStream(MemoryStream source, WaveFormat formatIn)
        {
            memoryStream = source;
            format = formatIn;
        }

        public override WaveFormat WaveFormat
        {
            get { return format; }
        }

        public override long Length
        {
            get { return long.MaxValue / 32; }
        }

        public override long Position
        {
            get
            {
                return memoryStream.Position;
            }
            set
            {
                memoryStream.Position = value;
            }
        }

        public override bool HasData(int count)
        {
            // Infinite loop
            return true;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = 0;
            while (read < count)
            {
                var required = count - read;
                var readThisTime = memoryStream.Read(buffer, offset + read, required);
                if (readThisTime < required)
                {
                    memoryStream.Position = 0;
                }

                if (memoryStream.Position >= memoryStream.Length)
                {
                    memoryStream.Position = 0;
                }
                read += readThisTime;
            }
            return read;
        }

        protected override void Dispose(bool disposing)
        {
            memoryStream.Dispose();
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Class to keep the samples and the size.
    /// </summary>
    public class WaveDataChunk
    {
        public uint Length;
        public short[] AudioSamples;

        /// <summary>
        /// Initializes a new data chunk with default values.
        /// </summary>
        public WaveDataChunk()
        {
            AudioSamples = new short[0];
            Length = 0;
        }
    }

    #region Not used

    //public class WaveHeader
    //{
    //    public string sGroupID; // RIFF
    //    public uint dwFileLength; // total file length minus 8, which is taken up by RIFF
    //    public string sRiffType; // always WAVE

    //    /// <summary>
    //    /// Initializes a WaveHeader object with the default values.
    //    /// </summary>
    //    public WaveHeader()
    //    {
    //        dwFileLength = 0;
    //        sGroupID = "RIFF";
    //        sRiffType = "WAVE";
    //    }
    //}

    //public class WaveFormatChunk
    //{
    //    public string sChunkID;         // Four bytes: "fmt "
    //    public uint dwChunkSize;        // Length of header in bytes
    //    public ushort wFormatTag;       // 1 (MS PCM)
    //    public ushort wChannels;        // Number of channels
    //    public uint dwSamplesPerSec;    // Frequency of the audio in Hz... 44100
    //    public uint dwAvgBytesPerSec;   // for estimating RAM allocation
    //    public ushort wBlockAlign;      // sample frame size, in bytes
    //    public ushort wBitsPerSample;    // bits per sample

    //    /// <summary>
    //    /// Initializes a format chunk with the following properties:
    //    /// Sample rate: 44100 Hz
    //    /// Channels: Stereo
    //    /// Bit depth: 16-bit
    //    /// </summary>
    //    public WaveFormatChunk()
    //    {
    //        sChunkID = "fmt ";
    //        dwChunkSize = 16;
    //        wFormatTag = 1;
    //        wChannels = 2;
    //        dwSamplesPerSec = 44100;
    //        wBitsPerSample = 16;
    //        wBlockAlign = (ushort)(wChannels * (wBitsPerSample / 8));
    //        dwAvgBytesPerSec = dwSamplesPerSec * wBlockAlign;
    //    }
    //}

    #endregion
}
