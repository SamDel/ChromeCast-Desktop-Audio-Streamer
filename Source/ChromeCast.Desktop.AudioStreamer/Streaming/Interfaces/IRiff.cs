using NAudio.Wave;

namespace ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces
{
    public interface IRiff
    {
        byte[] GetRiffHeader(WaveFormat format);
    }
}