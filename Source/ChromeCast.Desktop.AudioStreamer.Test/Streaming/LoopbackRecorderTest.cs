using System;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChromeCast.Desktop.AudioStreamer.Streaming;
using NAudio.Wave;
using ChromeCast.Desktop.AudioStreamer.Application;

namespace ChromeCast.Desktop.AudioStreamer.Test.Streaming
{
    [TestClass]
    public class LoopbackRecorderTest
    {
        AutoResetEvent asyncEvent;
        byte[] dataRecorded;

        [TestMethod]
        public void TestRecording()
        {
            asyncEvent = new AutoResetEvent(false);

            var loopbackRecorder = new LoopbackRecorder(new Logger());
            dataRecorded = null;
            loopbackRecorder.Start(null, DataAvailable);
            asyncEvent.WaitOne(100);

            Assert.IsTrue(dataRecorded != null && dataRecorded.Length > 0);

            dataRecorded = null;
            loopbackRecorder.StopRecording();
            asyncEvent.WaitOne(100);

            Assert.IsTrue(dataRecorded == null);
        }

        private void DataAvailable(byte[] dataRecordedIn, WaveFormat format)
        {
            dataRecorded = dataRecordedIn;
            asyncEvent.Set();
        }
    }
}
