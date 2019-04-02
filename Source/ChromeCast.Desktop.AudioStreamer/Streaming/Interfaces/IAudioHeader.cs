using ChromeCast.Desktop.AudioStreamer.Classes;
using NAudio.Wave;

namespace ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces
{
    public interface IAudioHeader
    {
        byte[] GetRiffHeader(WaveFormat format, uint dataSize = 0);
        byte[] GetMp3Header(WaveFormat format, SupportedStreamFormat streamFormat);
    }
}