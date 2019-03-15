using System;
using System.Linq;
using System.Collections.Generic;
using ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces;
using NAudio.Wave;
using CSCore.CoreAudioAPI;
using CSCore.SoundIn;
using CSCore.Streams;
using CSCore;
using System.Timers;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;
using System.Windows.Forms;

namespace ChromeCast.Desktop.AudioStreamer.Streaming
{
    public class LoopbackRecorder : ILoopbackRecorder
    {
        WasapiCapture soundIn;
        private Action<byte[], NAudio.Wave.WaveFormat> dataAvailableCallback;
        private Action clearMp3Buffer;
        private bool isRecording = false;
        IWaveSource convertedSource;
        SoundInSource soundInSource;
        NAudio.Wave.WaveFormat waveFormat;
        IMainForm mainForm;
        DateTime latestDataAvailable;
        System.Timers.Timer dataAvailableTimer;
        System.Timers.Timer getDevicesTimer;
        private ILogger logger;

        public LoopbackRecorder(ILogger loggerIn)
        {
            logger = loggerIn;
        }

        /// <summary>
        /// Start
        /// </summary>
        public void Start(IMainForm mainFormIn, Action<byte[], NAudio.Wave.WaveFormat> dataAvailableCallbackIn, Action clearMp3BufferIn)
        {
            if (mainFormIn == null || dataAvailableCallbackIn == null)
                return;

            getDevicesTimer = new System.Timers.Timer
            {
                Interval = 15000,
                Enabled = true
            };
            getDevicesTimer.Elapsed += new ElapsedEventHandler(DoStart);
            getDevicesTimer.Start();

            dataAvailableCallback = dataAvailableCallbackIn;
            clearMp3Buffer = clearMp3BufferIn;
            mainForm = mainFormIn;
            DoStart(null, null);
        }

        /// <summary>
        /// Do start.
        /// </summary>
        private void DoStart(object sender, ElapsedEventArgs e)
        {
            GetDevices();
            if (!isRecording)
            {
                StartRecording();
            }
        }

        /// <summary>
        /// Start recording.
        /// </summary>
        public void StartRecording()
        {
            if (isRecording)
                return;

            StartSilenceCheckTimer();
            StartRecordingDevice();
        }

        /// <summary>
        /// Get the recording device, the callback should be called when the recording device is there.
        /// </summary>
        public void StartRecordingDevice()
        {
            if (mainForm == null)
                return;

            mainForm.GetRecordingDevice();
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
                    mainForm.ShowWavMeterValue(dataToSend.ToArray());
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
        public void GetDevices()
        {
            if (mainForm == null)
                return;

            var devices = MMDeviceEnumerator.EnumerateDevices(DataFlow.Render, DeviceState.Active);
            if (devices.Count > 0)
            {
                var defaultDevice = MMDeviceEnumerator.DefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                mainForm.AddRecordingDevices(devices, defaultDevice);
            }
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
                logger.Log(Properties.Strings.MessageBox_NoRecordingDevices);
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
                soundIn.Stopped += OnRecordingStopped;
                soundIn.Start();

                var format = convertedSource.WaveFormat;
                waveFormat = NAudio.Wave.WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, format.SampleRate, format.Channels, format.BytesPerSecond, format.BlockAlign, format.BitsPerSample);
                isRecording = true;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing the recording device: {ex.Message}");
                logger.Log(ex, "StartRecordingSetDevice");
            }

            return false;
        }

        /// <summary>
        /// Recording has stopped.
        /// </summary>
        private void OnRecordingStopped(object sender, RecordingStoppedEventArgs e)
        {
            if (isRecording)
            {
                logger.Log("Recording Stopped");
                isRecording = false;
            }
        }
        
        /// <summary>
        /// Start a timer that checks for silence (nothing recorded).
        /// </summary>
        private void StartSilenceCheckTimer()
        {
            if (dataAvailableTimer == null)
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
                logger.Log($"Check For Silence: Send Silence ({(DateTime.Now - latestDataAvailable).TotalSeconds})");
                clearMp3Buffer();
            }
            if ((DateTime.Now - latestDataAvailable).TotalSeconds > 2)
            {
                logger.Log($"Check For Silence: {(DateTime.Now - latestDataAvailable).TotalSeconds}");
                clearMp3Buffer();
            }
        }
    }
}
