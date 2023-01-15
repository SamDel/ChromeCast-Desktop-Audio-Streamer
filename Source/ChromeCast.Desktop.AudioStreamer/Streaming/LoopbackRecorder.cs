using System;
using System.Linq;
using System.Collections.Generic;
using ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces;
using NAudio.Wave;
using System.Timers;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;
using System.Threading;
using System.Diagnostics;
using NAudio.CoreAudioApi;

namespace ChromeCast.Desktop.AudioStreamer.Streaming
{
    public class LoopbackRecorder : ILoopbackRecorder
    {
        WasapiCapture soundIn;
        private Action<byte[], WaveFormat> dataAvailableCallback;
        private bool isRecording = false;
        WaveFormat waveFormat;
        IMainForm mainForm;
        DateTime latestDataAvailable;
        System.Timers.Timer dataAvailableTimer;
        System.Timers.Timer getDevicesTimer;
        private readonly ILogger logger;
        Thread eventThread;
        BufferBlock bufferCaptured, bufferSend;
        readonly object bufferSwapSync = new();

        public LoopbackRecorder(ILogger loggerIn)
        {
            logger = loggerIn;
        }

        /// <summary>
        /// Start
        /// </summary>
        public void Start(IMainForm mainFormIn, Action<byte[], WaveFormat> dataAvailableCallbackIn, Action clearMp3BufferIn)
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
        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            if (soundIn == null || soundIn.WaveFormat == null)
                return;

            latestDataAvailable = DateTime.Now;

            if (dataAvailableCallback != null)
            {
                lock (bufferSwapSync)
                {
                    var currentBuffer = bufferCaptured;
                    currentBuffer.Add(e.Buffer, e.BytesRecorded);
                }
            }
        }

        /// <summary>
        /// Stop recording.
        /// </summary>
        public void StopRecording()
        {
            isRecording = false;

            if (soundIn == null)
                return;

            soundIn?.StopRecording();
            soundIn?.Dispose();
            soundIn = null;
        }

        /// <summary>
        /// Get the recording device.
        /// </summary>
        public void GetDevices()
        {
            if (mainForm == null)
                return;

            var enumerator = new MMDeviceEnumerator();
            var enumDevices = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
            var devices = ConvertDevices(enumDevices);
            if (devices.Count > 0)
            {
                var defaultDeviceEndpoint = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                var defaultDevice = ConvertDevice(defaultDeviceEndpoint);
                mainForm.AddRecordingDevices(devices, defaultDevice);
            }
        }

        private static List<RecordingDevice> ConvertDevices(MMDeviceCollection devices)
        {
            List<RecordingDevice> result = new();
            foreach (var device in devices)
            {
                result.Add(ConvertDevice(device));
            }
            return result;
        }

        private static RecordingDevice ConvertDevice(MMDevice device)
        {
            if (device == null)
                return null;

            return new RecordingDevice { 
                Name = device.FriendlyName, 
                ID = device.ID, 
                Flow = device.DataFlow, 
                SampleRate = device.AudioClient.MixFormat.SampleRate, 
                Channels = device.AudioClient.MixFormat.Channels };
        }

        /// <summary>
        /// Start recording on the device in the parameter.
        /// </summary>
        /// <param name="recordingDevice">the device to start recording</param>
        /// <returns>true if the recording is started, or false</returns>
        public bool StartRecordingSetDevice(RecordingDevice audioDevice)
        {
            if (audioDevice == null)
            {
                logger.Log(Properties.Strings.MessageBox_NoRecordingDevices);
                return false;
            }

            try
            {
                MMDevice recordingDevice = null;
                var enumerator = new MMDeviceEnumerator();
                var enumDevices = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
                foreach (var enumDevice in enumDevices)
                {
                    if (enumDevice.ID == audioDevice.ID)
                        recordingDevice = enumDevice;
                }

                if (recordingDevice == null)
                    return false;

                if (recordingDevice.DataFlow == DataFlow.Render)
                {
                    soundIn = new WasapiLoopbackCapture(recordingDevice);
                }
                else
                {
                    soundIn = new WasapiCapture(recordingDevice);
                }


                var selectedFormat = mainForm.GetSelectedStreamFormat();
                var convertMultiChannelToStereo = mainForm.GetConvertMultiChannelToStereo();
                var nrChannels = convertMultiChannelToStereo ? soundIn.WaveFormat.Channels : 2;
                switch (selectedFormat)
                {
                    case Classes.SupportedStreamFormat.Wav:
                        soundIn.WaveFormat = new WaveFormat(44100, 16, nrChannels);
                        break;
                    case Classes.SupportedStreamFormat.Mp3_320:
                    case Classes.SupportedStreamFormat.Mp3_128:
                        soundIn.WaveFormat = new WaveFormat(soundIn.WaveFormat.SampleRate, 16, 2);
                        break;
                    case Classes.SupportedStreamFormat.Wav_16bit:
                        soundIn.WaveFormat = new WaveFormat(soundIn.WaveFormat.SampleRate, 16, nrChannels);
                        break;
                    case Classes.SupportedStreamFormat.Wav_24bit:
                        soundIn.WaveFormat = new WaveFormat(soundIn.WaveFormat.SampleRate, 24, nrChannels);
                        break;
                    case Classes.SupportedStreamFormat.Wav_32bit:
                        soundIn.WaveFormat = new WaveFormat(soundIn.WaveFormat.SampleRate, 32, nrChannels);
                        break;
                    default:
                        break;
                }
                waveFormat = soundIn.WaveFormat;
                logger.Log($"Stream format set to {waveFormat.Encoding} {waveFormat.SampleRate} {waveFormat.BitsPerSample} bit");
                soundIn.DataAvailable += OnDataAvailable;
                soundIn.RecordingStopped += OnRecordingStopped;
                soundIn.StartRecording();
                isRecording = true;

                var bytesPerSecond = soundIn.WaveFormat.SampleRate * soundIn.WaveFormat.Channels * (soundIn.WaveFormat.BitsPerSample / 8);
                bufferCaptured = new BufferBlock() { Data = new byte[bytesPerSecond / 2] };
                bufferSend = new BufferBlock() { Data = new byte[bytesPerSecond / 2] };

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
        private void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            if (isRecording)
            {
                logger.Log("Recording Stopped");
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
            soundIn?.StopRecording();
            soundIn?.Dispose();
            soundIn = null;
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
