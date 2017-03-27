using System.IO;
using System.Text;
using NAudio.Wave;
using ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces;

namespace ChromeCast.Desktop.AudioStreamer.Streaming
{
    public class Riff : IRiff
    {
        /// <summary>
        /// Generate a header for a maximum length WAV stream.
        /// </summary>
        public byte[] GetRiffHeader(WaveFormat format)
        {
            var dataSize = (uint)0;
            uint chunkSize = dataSize;
            uint factChunkSize = 4;
            uint numberOfSamples = (uint)((dataSize * 8) / format.BitsPerSample / format.Channels);

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
    }
}
