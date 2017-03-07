using System.IO;
using System.Text;
using NAudio.Wave;
using ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces;

namespace ChromeCast.Desktop.AudioStreamer.Streaming
{
    public class Riff : IRiff
    {
        public byte[] GetRiffHeader(WaveFormat format)
        {
            var riffHeaderStream = new MemoryStream();

            GetWaveFileHeader(riffHeaderStream, format);

            return riffHeaderStream.ToArray();
        }

        private void GetWaveFileHeader(Stream outStream, WaveFormat format)
        {
            var dataSize = uint.MaxValue - 50;
            uint chunkSize = dataSize;
            uint factChunkSize = 4;
            uint numberOfSamples = (uint)((dataSize * 8) / format.BitsPerSample / format.Channels);

            var writer = new BinaryWriter(outStream, Encoding.UTF8);
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
        }
    }
}
