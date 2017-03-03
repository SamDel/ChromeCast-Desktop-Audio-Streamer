using System.Linq;
using System.Collections.Generic;
using NAudio.Wave;
using ChromeCast.Desktop.AudioStreamer.Application;

namespace ChromeCast.Desktop.AudioStreamer.Streaming
{
    public class LoopbackRecorder
    {
        private ApplicationLogic application;
        private IWaveIn waveIn;
        private bool isRecording = false;

        public LoopbackRecorder(ApplicationLogic app)
        {
            application = app;
        }

        public void StartRecording()
        {
            if (isRecording) return;

            waveIn = new WasapiLoopbackCapture();
            waveIn.DataAvailable += OnDataAvailable;
            waveIn.RecordingStopped += OnRecordingStopped;
            waveIn.StartRecording();

            isRecording = true;
        }

        private void OnDataAvailable(object sender, WaveInEventArgs eventArgs)
        {
            var dataToSend = new List<byte>();

            dataToSend.AddRange(eventArgs.Buffer.Take(eventArgs.BytesRecorded).ToArray());

            application.OnRecordingDataAvailable(dataToSend.ToArray(), waveIn.WaveFormat);
        }

        public void StopRecording()
        {
            isRecording = false;
            if (waveIn != null)
            {
                waveIn.StopRecording();
            }
        }

        void OnRecordingStopped(object sender, StoppedEventArgs eventArgs)
        {
            if (waveIn != null)
            {
                waveIn.Dispose();
                waveIn = null;
            }
            isRecording = false;

            if (eventArgs.Exception != null)
            {
                throw eventArgs.Exception;
            }
        }
    }
}