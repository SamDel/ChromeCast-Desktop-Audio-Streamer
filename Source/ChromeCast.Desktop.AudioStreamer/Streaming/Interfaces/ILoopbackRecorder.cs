using System;
using NAudio.Wave;

namespace ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces
{
    public interface ILoopbackRecorder
    {
        void StartRecording(Action<byte[], WaveFormat> dataAvailableCallback);
        void StopRecording();
    }
}