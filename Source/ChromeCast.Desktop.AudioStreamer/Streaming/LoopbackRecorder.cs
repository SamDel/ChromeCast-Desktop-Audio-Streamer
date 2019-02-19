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
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;

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
        private ILogger logger;

        public LoopbackRecorder(ILogger loggerIn)
        {
            logger = loggerIn;
        }

        /// <summary>
        /// Start recording.
        /// </summary>
        public void StartRecording(Action<byte[], NAudio.Wave.WaveFormat> dataAvailableCallbackIn)
        {
            if (isRecording)
                return;

            dataAvailableCallback = dataAvailableCallbackIn;

            StartSilenceCheckTimer();
            StartRecordingDevice();
            isRecording = true;
        }

        /// <summary>
        /// Get the recording device, the callback should be called when the recording device is there.
        /// </summary>
        public void StartRecordingDevice()
        {
            if (mainForm == null)
                return;

            mainForm.GetRecordingDevice(StartRecordingSetDevice);
        }

        /// <summary>
        /// New recording data is available, distribute the data.
        /// </summary>
        private void OnDataAvailable(object sender, DataAvailableEventArgs e)
        {
            if (convertedSource == null || convertedSource.WaveFormat == null)
                return;

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

        /// <summary>
        /// Stop recording.
        /// </summary>
        public void StopRecording()
        {
            if (soundIn == null)
                return;

            isRecording = false;
            soundIn.Stop();
        }

        /// <summary>
        /// Get the recording device.
        /// </summary>
        public void GetDevices(IMainForm mainFormIn)
        {
            if (mainFormIn == null)
                return;

            mainForm = mainFormIn;
            var defaultDevice = MMDeviceEnumerator.DefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            var devices = MMDeviceEnumerator.EnumerateDevices(DataFlow.Render, DeviceState.Active);
            mainForm.AddRecordingDevices(devices, defaultDevice);
        }

        /// <summary>
        /// Start recording on the device in the parameter.
        /// </summary>
        /// <param name="recordingDevice">the device to start recording</param>
        /// <returns>true if the recording is started, or false</returns>
        public bool StartRecordingSetDevice(MMDevice recordingDevice)
        {
            if (recordingDevice == null)
            {
                MessageBox.Show(Properties.Strings.MessageBox_NoRecordingDevices);
                Console.WriteLine("No devices found.");
                return false;
            }

            try
            {
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
                return true;
            }
            catch (Exception ex)
            {
                logger.Log($"ex : {ex.Message}");
                Task.Delay(10000).Wait();
            }

            return false;
        }

        /// <summary>
        /// Start a timer that checks for silence (nothing recorded).
        /// </summary>
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

        /// <summary>
        /// If there's nothing recorded for a while, stream silence to keep the connection alive.
        /// </summary>
        private void OnCheckForSilence(object sender, ElapsedEventArgs e)
        {
            if (dataAvailableCallback == null)
                return;

            if ((DateTime.Now - latestDataAvailable).TotalSeconds > 5)
            {
                latestDataAvailable = DateTime.Now;
                dataAvailableCallback(Properties.Resources.silenceWav, waveFormat);
            }
        }
    }
}