using ChromeCast.Desktop.AudioStreamer.Streaming;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChromeCast.Desktop.AudioStreamer.Test.Streaming
{
    [TestClass]
    public class RiffTest
    {
        [TestMethod]
        public void TestRiffHeader()
        {
            var riffHeader = new AudioHeader().GetRiffHeader(new NAudio.Wave.WaveFormat(48000, 2));

            Assert.AreEqual(58, riffHeader.Length);
        }
    }
}
