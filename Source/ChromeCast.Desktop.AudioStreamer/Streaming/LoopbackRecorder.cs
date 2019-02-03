using System;
using System.Linq;
using System.Collections.Generic;
using ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces;
using NAudio.Wave;
using CSCore.CoreAudioAPI;
using CSCore.SoundIn;
using CSCore.Streams;
using CSCore;
using System.Windows.Forms;
using System.Timers;
using System.Threading.Tasks;

namespace ChromeCast.Desktop.AudioStreamer.Streaming
{
    public class LoopbackRecorder : ILoopbackRecorder
    {
        WasapiCapture soundIn;
        private Action<byte[], NAudio.Wave.WaveFormat> dataAvailableCallback;
        private bool isRecording = false;
        IWaveSource convertedSource;
        SoundInSource soundInSource;
        NAudio.Wave.WaveFormat waveFormat;
        IMainForm mainForm;
        DateTime latestDataAvailable;
        System.Timers.Timer dataAvailableTimer;

        public void StartRecording(Action<byte[], NAudio.Wave.WaveFormat> dataAvailableCallbackIn)
        {
            if (isRecording)
                return;

            dataAvailableCallback = dataAvailableCallbackIn;

            StartSilenceCheckTimer();
            StartRecordingDevice();
            isRecording = true;
        }

        public void StartRecordingDevice()
        {
            mainForm.GetRecordingDevice(StartRecordingSetDevice);
        }

        private void OnDataAvailable(object sender, DataAvailableEventArgs e)
        {
            latestDataAvailable = DateTime.Now;

            if (dataAvailableCallback != null)
            {
                byte[] buffer = new byte[convertedSource.WaveFormat.BytesPerSecond / 2];
                int read;

                while ((read = convertedSource.Read(buffer, 0, buffer.Length)) > 0)
                {
                    var dataToSend = new List<byte>();
                    dataToSend.AddRange(buffer.Take(read).ToArray());
                    dataAvailableCallback(dataToSend.ToArray(), waveFormat);
                }
            }
        }

        public void StopRecording()
        {
            isRecording = false;
            if (soundIn != null)
            {
                soundIn.Stop();
            }
        }

        private void OnRecordingStopped(object sender, CSCore.StoppedEventArgs eventArgs)
        {
            if (soundIn != null)
            {
                soundIn.Dispose();
                soundIn = null;
            }
            isRecording = false;

            if (eventArgs.Exception != null)
            {
                throw eventArgs.Exception;
            }
        }

        public void GetDevices(IMainForm mainFormIn)
        {
            mainForm = mainFormIn;
            var defaultDevice = MMDeviceEnumerator.DefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            var devices = MMDeviceEnumerator.EnumerateDevices(DataFlow.Render, DeviceState.Active);
            mainForm.AddRecordingDevices(devices, defaultDevice);
        }

        public void StartRecordingSetDevice(MMDevice recordingDevice)
        {
            if (recordingDevice == null)
            {
                MessageBox.Show(Properties.Strings.MessageBox_NoRecordingDevices);
                Console.WriteLine("No devices found.");
                return;
            }

            soundIn = new CSCore.SoundIn.WasapiLoopbackCapture
            {
                Device = recordingDevice
            };

            soundIn.Initialize();
            soundInSource = new SoundInSource(soundIn) { FillWithZeros = false };
            convertedSource = soundInSource.ChangeSampleRate(44100).ToSampleSource().ToWaveSource(16);
            convertedSource = convertedSource.ToStereo();
            soundInSource.DataAvailable += OnDataAvailable;
            soundIn.Start();

            var format = convertedSource.WaveFormat;
            waveFormat = NAudio.Wave.WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, format.SampleRate, format.Channels, format.BytesPerSecond, format.BlockAlign, format.BitsPerSample);
        }

        private void StartSilenceCheckTimer()
        {
            latestDataAvailable = DateTime.Now;
            dataAvailableTimer = new System.Timers.Timer
            {
                Interval = 1000,
                Enabled = true
            };
            dataAvailableTimer.Elapsed += new ElapsedEventHandler(OnCheckForSilence);
            dataAvailableTimer.Start();
        }

        private void OnCheckForSilence(object sender, ElapsedEventArgs e)
        {
            if ((DateTime.Now - latestDataAvailable).TotalSeconds > 1)
            {
                Task.Run(() => {
                    dataAvailableCallback(Properties.Resources.silenceWav, waveFormat);
                });
            }
        }
    }
}