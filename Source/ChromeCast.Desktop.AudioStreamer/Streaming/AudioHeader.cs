using System.IO;
using System.Text;
using NAudio.Wave;
using ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Classes;

namespace ChromeCast.Desktop.AudioStreamer.Streaming
{
    public class AudioHeader : IAudioHeader
    {
        /// <summary>
        /// Generate a header for a maximum length WAV stream.
        /// </summary>
        public byte[] GetRiffHeader(WaveFormat format)
        {
            if (format == null)
                return new byte[0];

            var dataSize = (uint)0;
            uint chunkSize = dataSize;
            uint factChunkSize = 4;
            uint numberOfSamples = (uint)(dataSize * 8 / format.BitsPerSample / format.Channels);

            var riffHeaderStream = new MemoryStream();
            var writer = new BinaryWriter(riffHeaderStream, Encoding.UTF8);

            writer.Write(Encoding.UTF8.GetBytes("RIFF"));
            writer.Write(chunkSize);
            writer.Write(Encoding.UTF8.GetBytes("WAVE"));
            writer.Write(Encoding.UTF8.GetBytes("fmt "));
            format.Serialize(writer);
            writer.Write(Encoding.UTF8.GetBytes("fact"));
            writer.Write(factChunkSize);
            writer.Write(numberOfSamples);
            writer.Write(Encoding.UTF8.GetBytes("data"));
            writer.Write(dataSize);

            return riffHeaderStream.ToArray();
        }

        /// <summary>
        /// Generate a mp3 header for a mp3 stream.
        /// see: http://www.mp3-tech.org/programmer/frame_header.html
        /// </summary>
        /// <param name="format">the format of the stream</param>
        /// <returns>a mp3 header</returns>
        public byte[] GetMp3Header(WaveFormat format, SupportedStreamFormat streamFormat)
        {
            if (format == null)
                return new byte[0];

            var riffHeaderStream = new MemoryStream();
            var writer = new BinaryWriter(riffHeaderStream, Encoding.UTF8);

            //Write header: AAAAAAAA AAABBCCD EEEEFFGH IIJJKLMM
            writer.Write(new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }); // A: all bits must be set
            writer.Write(new byte[] { 1, 1 }); // B: MPEG Version 1 (ISO/IEC 11172-3)
            writer.Write(new byte[] { 0, 1 }); // C: Layer III
            writer.Write(new byte[] { 1 }); // D: 1 - Not protected

            if (streamFormat.Equals(SupportedStreamFormat.Mp3_128))
                //                                                                              V1,L3
                writer.Write(new byte[] { 1, 0, 0, 1 }); // E: bitrate: 1001 	288 	160 	128 	144 	80
            else
                writer.Write(new byte[] { 1, 1, 1, 0 }); // E: bitrate: 1110 	448 	384 	320 	256 	160

            if (format.SampleRate == 44100)
                writer.Write(new byte[] { 0, 0 }); // F: MPEG1 - 44100 Hz
            else if (format.SampleRate == 48000)
                writer.Write(new byte[] { 0, 1 }); // F: MPEG1 - 48000 Hz
            else
                writer.Write(new byte[] { 1, 0 }); // F: MPEG1 - 32000  Hz

            writer.Write(new byte[] { 0 }); // G: 0 - frame is not padded
            writer.Write(new byte[] { 0 }); // H: Private bit. This one is only informative. 
            writer.Write(new byte[] { 0, 0 }); // I: Stereo
            writer.Write(new byte[] { 0, 0 }); // J: Mode extension (Only used in Joint stereo) 
            writer.Write(new byte[] { 0 }); // K: Copyright - Audio is not copyrighted
            writer.Write(new byte[] { 0 }); // L: Original - Copy of original media
            writer.Write(new byte[] { 0, 0 }); // M: Emphasis - none

            return riffHeaderStream.ToArray();
        }
    }
}
