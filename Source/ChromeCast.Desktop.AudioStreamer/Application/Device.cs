using System;
using System.Windows.Forms;
using Rssdp;
using NAudio.Wave;
using ChromeCast.Desktop.AudioStreamer.Communication;
using ChromeCast.Desktop.AudioStreamer.Communication.Classes;
using ChromeCast.Desktop.AudioStreamer.Streaming;
using ChromeCast.Desktop.AudioStreamer.UserControls;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class Device
    {
        public DiscoveredSsdpDevice DiscoveredSsdpDevice;
        public SsdpDevice SsdpDevice;
        public DeviceConnection DeviceConnection;
        public DeviceCommunication DeviceCommunication;
        public StreamingConnection StreamingConnection;
        public DeviceControl DeviceControl;
        public MenuItem MenuItem;
        public DeviceState DeviceState;
        private ApplicationLogic application;
        private Volume volumeSetting;
        private DateTime lastVolumeChange;
        private int reduceLagCounter = 0;

        public Device(ApplicationLogic app, DiscoveredSsdpDevice discoveredSsdpDevice, SsdpDevice ssdpDevice)
        {
            application = app;
            DiscoveredSsdpDevice = discoveredSsdpDevice;
            SsdpDevice = ssdpDevice;
            DeviceConnection = new DeviceConnection(this, application);
            DeviceCommunication = new DeviceCommunication(this, application);
            DeviceState = DeviceState.NotConnected;

            volumeSetting = new Volume
            {
                controlType = "attenuation",
                level = 0.0f,
                muted = false,
                stepInterval = 0.05f
            };
        }

        public void OnClickDeviceButton(object sender, EventArgs e)
        {
            switch (DeviceState)
            {
                case DeviceState.Buffering:
                case DeviceState.Playing:
                    DeviceCommunication.PauseMedia();
                    break;
                case DeviceState.LaunchingApplication:
                case DeviceState.LaunchedApplication:
                case DeviceState.LoadingMedia:
                case DeviceState.Idle:
                case DeviceState.Paused:
                    DeviceCommunication.LoadMedia();
                    break;
                case DeviceState.NotConnected:
                case DeviceState.ConnectError:
                case DeviceState.Closed:
                    DeviceCommunication.LaunchAndLoadMedia();
                    break;
                case DeviceState.Disposed:
                    break;
                default:
                    break;
            }
        }

        public void OnRecordingDataAvailable(byte[] dataToSend, WaveFormat format, int reduceLagThreshold)
        {
            if (StreamingConnection != null)
            {
                if (StreamingConnection.Socket.Connected)
                {
                    if (reduceLagThreshold < 1000)
                    {
                        reduceLagCounter++;
                        if (reduceLagCounter > reduceLagThreshold)
                        {
                            reduceLagCounter = 0;
                            return;
                        }
                    }

                    if (!StreamingConnection.IsMaxWavSizeReached(dataToSend.Length))
                    {
                        StreamingConnection.SendData(dataToSend, format);
                    }
                    else
                    {
                        DeviceCommunication.LoadMedia();
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("Connection closed from {0}", StreamingConnection.Socket.RemoteEndPoint));
                    StreamingConnection = null;
                }
            }
        }

        public void OnGetStatus()
        {
            DeviceCommunication.GetMediaStatus();
        }

        public void SetDeviceState(DeviceState state, string text = null)
        {
            DeviceState = state;
            DeviceControl.SetStatus(state, text);
        }

        public bool IsConnected()
        {
            return !(DeviceState.Equals(DeviceState.NotConnected) ||
                DeviceState.Equals(DeviceState.ConnectError) ||
                DeviceState.Equals(DeviceState.Closed));
        }

        public bool IsPlaying()
        {
            return DeviceState == DeviceState.LoadingMedia ||
                    DeviceState == DeviceState.Buffering ||
                    DeviceState == DeviceState.Playing;
        }

        public void OnVolumeUpdate(Volume volume)
        {
            volumeSetting = volume;
            DeviceControl.OnVolumeUpdate(volume);
        }

        public void VolumeSet(float level)
        {
            if (!IsConnected())
                return;

            if (lastVolumeChange != null && DateTime.Now.Ticks - lastVolumeChange.Ticks < 1000)
                return;

            lastVolumeChange = DateTime.Now;

            if (volumeSetting.level > level)
                while (volumeSetting.level > level) volumeSetting.level -= volumeSetting.stepInterval;
            if (volumeSetting.level < level)
                while (volumeSetting.level < level) volumeSetting.level += volumeSetting.stepInterval;
            if (level > 1) level = 1;
            if (level < 0) level = 0;

            volumeSetting.level = level;
            DeviceCommunication.VolumeSet(volumeSetting);
        }

        public void VolumeUp()
        {
            if (!IsConnected())
                return;

            VolumeSet(volumeSetting.level + 0.05f);
        }

        public void VolumeDown()
        {
            if (!IsConnected())
                return;

            VolumeSet(volumeSetting.level - 0.05f);
        }

        public void VolumeMute()
        {
            if (!IsConnected())
                return;

            DeviceCommunication.VolumeMute(!volumeSetting.muted);
        }
    }
}
