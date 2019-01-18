using System.IO;
using NAudio.Lame;
using NAudio.Wave;

namespace ChromeCast.Desktop.AudioStreamer.Classes
{
    public class Mp3Stream
    {
        private MemoryStream Output { get; set; }
        private LameMP3FileWriter Writer { get; set; }

        public Mp3Stream(WaveFormat format, SupportedStreamFormat formatSelected)
        {
            Output = new MemoryStream();
            var bitRate = 128;
            if (formatSelected.Equals(SupportedStreamFormat.Mp3_320))
                bitRate = 320;

            Writer = new LameMP3FileWriter(Output, format, bitRate);
        }

        public void Encode(byte[] buffer)
        {
            Writer.Write(buffer, 0, buffer.Length);
        }

        public byte[] Read()
        {
            var byteArray =  Output.ToArray();
            Output.SetLength(0);
            return byteArray;
        }
    }
}
