using NAudio.CoreAudioApi;

namespace ChromeCast.Desktop.AudioStreamer.Streaming
{
    public class RecordingDevice
    {
        public override string ToString()
        {
            return Name;
        }
        public string Name { get; set; }
        public string ID { get; set; }
        public DataFlow Flow { get; set; }
        public int SampleRate { get; set; }
        public int Channels { get; set; }
    }
}