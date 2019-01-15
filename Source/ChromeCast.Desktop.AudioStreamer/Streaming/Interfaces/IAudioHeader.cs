using ChromeCast.Desktop.AudioStreamer.Classes;
using NAudio.Wave;

namespace ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces
{
    public interface IAudioHeader
    {
        byte[] GetRiffHeader(WaveFormat format);
        byte[] GetMp3Header(WaveFormat format, SupportedStreamFormat streamFormat);
    }
}