using System;
using System.Linq;
using System.Collections.Generic;
using NAudio.Wave;
using ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces;

namespace ChromeCast.Desktop.AudioStreamer.Streaming
{
    public class LoopbackRecorder : ILoopbackRecorder
    {
        private IWaveIn waveIn;
        private Action<byte[], WaveFormat> dataAvailableCallback;
        private bool isRecording = false;

        public void StartRecording(Action<byte[], WaveFormat> dataAvailableCallbackIn)
        {
            if (isRecording)
                return;

            dataAvailableCallback = dataAvailableCallbackIn;
            waveIn = new WasapiLoopbackCapture();
            waveIn.DataAvailable += OnDataAvailable;
            waveIn.RecordingStopped += OnRecordingStopped;
            waveIn.StartRecording();

            isRecording = true;
        }

        private void OnDataAvailable(object sender, WaveInEventArgs eventArgs)
        {
            if (dataAvailableCallback != null)
            {
                var dataToSend = new List<byte>();
                dataToSend.AddRange(eventArgs.Buffer.Take(eventArgs.BytesRecorded).ToArray());
                dataAvailableCallback(dataToSend.ToArray(), waveIn?.WaveFormat);
            }
        }

        public void StopRecording()
        {
            isRecording = false;
            if (waveIn != null)
            {
                waveIn.StopRecording();
            }
        }

        private void OnRecordingStopped(object sender, StoppedEventArgs eventArgs)
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