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
using System.Threading;
using System.Diagnostics;

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
        System.Timers.Timer getDevicesTimer;
        private readonly ILogger logger;
        Thread eventThread;
        BufferBlock bufferCaptured, bufferSend;
        readonly object bufferSwapSync = new object();

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
                int read;

                lock (bufferSwapSync)
                {
                    var currentBuffer = bufferCaptured;
                    var spaceLeft = bufferCaptured.Data.Length - bufferCaptured.Used;

                    while (spaceLeft > 0 && (read = convertedSource.Read(currentBuffer.Data, currentBuffer.Used, spaceLeft)) > 0)
                    {
                        spaceLeft -= read;
                        currentBuffer.Used += read;
                    }
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
            soundIn?.Stop();
        }

        /// <summary>
        /// Get the recording device.
        /// </summary>
        public void GetDevices()
        {
            if (mainForm == null)
                return;

            var devices = MMDeviceEnumerator.EnumerateDevices(DataFlow.All, DeviceState.Active);
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
                if (recordingDevice.DataFlow == DataFlow.Render)
                {
                    soundIn = new CSCore.SoundIn.WasapiLoopbackCapture
                    {
                        Device = recordingDevice
                    };
                }
                else
                {
                    soundIn = new CSCore.SoundIn.WasapiCapture
                    {
                        Device = recordingDevice
                    };
                }


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
                bufferCaptured = new BufferBlock() { Data = new byte[convertedSource.WaveFormat.BytesPerSecond / 2] };
                bufferSend = new BufferBlock() { Data = new byte[convertedSource.WaveFormat.BytesPerSecond / 2] };

                eventThread = new Thread(EventThread)
                {
                    Name = "Loopback Event Thread",
                    IsBackground = true
                };
                eventThread.Start(new WeakReference<LoopbackRecorder>(this));

                return true;
            }
            catch (Exception ex)
            {
                logger.Log(ex, "Error initializing the recording device:");
            }

            return false;
        }

        /// <summary>
        /// Thread for the data captured events.
        /// </summary>
        /// <param name="param"></param>
        private static void EventThread(object param)
        {
            var thisRef = (WeakReference<LoopbackRecorder>)param;
            try
            {
                while (true)
                {
                    if (!thisRef.TryGetTarget(out LoopbackRecorder recorder) || recorder == null)
                    {
                        // Instance is dead
                        return;
                    }

                    if (!recorder.isRecording)
                    {
                        return;
                    }

                    recorder.SwapBuffer();
                    if (recorder.bufferSend.Used > 0)
                    {
                        var bytes = new List<byte>();
                        bytes.AddRange(recorder.bufferSend.Data.Take(recorder.bufferSend.Used).ToArray());
                        recorder.dataAvailableCallback(bytes.ToArray(), recorder.waveFormat);
                        recorder.bufferSend.Used = 0;
                        recorder.mainForm.ShowWavMeterValue(bytes.ToArray());
                        //NAudio.CoreAudioApi.MMDeviceEnumerator enumerator = new NAudio.CoreAudioApi.MMDeviceEnumerator();
                        //var device = enumerator.GetDefaultAudioEndpoint(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.Role.Multimedia);
                        //Console.WriteLine(device.AudioMeterInformation.MasterPeakValue);
                    }

                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
            }
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
                var silence = new WavGenerator().GetSilenceBytes(1);
                dataAvailableCallback(silence, waveFormat);
                logger.Log($"Check For Silence: Send Silence ({(DateTime.Now - latestDataAvailable).TotalSeconds})");
            }
            if ((DateTime.Now - latestDataAvailable).TotalSeconds > 2)
            {
                logger.Log($"Check For Silence: {(DateTime.Now - latestDataAvailable).TotalSeconds}");
            }
        }

        private void SwapBuffer()
        {
            lock (bufferSwapSync)
            {
                var tmp = bufferCaptured;
                bufferCaptured = bufferSend;
                bufferSend = tmp;
            }
        }

        public void Dispose()
        {
            soundIn?.Stop();
            soundIn?.Dispose();
            soundIn = null;
            convertedSource?.Dispose();
            convertedSource = null;
            soundInSource?.Dispose();
            soundInSource = null;
            dataAvailableTimer?.Close();
            dataAvailableTimer?.Dispose();
            getDevicesTimer?.Close();
            getDevicesTimer?.Dispose();
        }

        public void Restart()
        {
            StopRecording();
            DoStart(null, null);
        }
    }
}
