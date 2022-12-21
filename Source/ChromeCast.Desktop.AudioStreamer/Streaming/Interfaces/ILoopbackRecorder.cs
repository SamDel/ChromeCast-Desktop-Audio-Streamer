using System;
using NAudio.Wave;

namespace ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces
{
    public interface ILoopbackRecorder
    {
        void Start(IMainForm mainForm, Action<byte[], WaveFormat> dataAvailableCallback, Action clearMp3Buffer);
        void StartRecordingDevice();
        bool StartRecordingSetDevice(RecordingDevice device);
        void StopRecording();
        void Dispose();
        void Restart();
    }
}