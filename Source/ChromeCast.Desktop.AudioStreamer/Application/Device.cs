using System;
using System.Net.Sockets;
using System.Windows.Forms;
using Rssdp;
using NAudio.Wave;
using Microsoft.Practices.Unity;
using ChromeCast.Desktop.AudioStreamer.Communication;
using ChromeCast.Desktop.AudioStreamer.Communication.Classes;
using ChromeCast.Desktop.AudioStreamer.UserControls;
using ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Communication.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Classes;
using ChromeCast.Desktop.AudioStreamer.Streaming;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class Device : IDevice
    {
        private IDeviceCommunication deviceCommunication;
        private IStreamingConnection streamingConnection;
        private DiscoveredSsdpDevice discoveredSsdpDevice;
        private SsdpDevice ssdpDevice;
        private DeviceControl deviceControl;
        private MenuItem menuItem;
        private Volume volumeSetting;
        private DateTime lastVolumeChange;

        public Device(IDeviceCommunication deviceCommunicationIn)
        {
            deviceCommunication = deviceCommunicationIn;
            deviceCommunication.SetCallback(SetDeviceState, OnVolumeUpdate, IsConnected, GetHost);
            volumeSetting = new Volume
            {
                controlType = "attenuation",
                level = 0.0f,
                muted = false,
                stepInterval = 0.05f
            };
        }

        public void SetDiscoveredDevices(DiscoveredSsdpDevice discoveredSsdpDeviceIn, SsdpDevice ssdpDeviceIn)
        {
            discoveredSsdpDevice = discoveredSsdpDeviceIn;
            ssdpDevice = ssdpDeviceIn;
        }

        public void OnClickDeviceButton(object sender, EventArgs e)
        {
            deviceCommunication.OnPlayPause_Click();
        }

        public void Start()
        {
            deviceCommunication.LoadMedia();
        }

        public void OnRecordingDataAvailable(byte[] dataToSend, WaveFormat format, int reduceLagThreshold, SupportedStreamFormat streamFormat)
        {
            if (streamingConnection != null)
            {
                if (streamingConnection.IsConnected())
                {
                    var state = deviceCommunication.GetDeviceState();
                    if (state != DeviceState.Paused && state != DeviceState.Closed)
                    {
                        streamingConnection.SendData(dataToSend, format, reduceLagThreshold, streamFormat);

                        //TODO: updates only the shown state, not the communication state!?
                        if (state != DeviceState.Buffering && state != DeviceState.Playing)
                            SetDeviceState(DeviceState.Playing, "");
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("Connection closed from {0}", streamingConnection.GetRemoteEndPoint()));
                    streamingConnection = null;
                }
            }
        }

        public void OnGetStatus()
        {
            deviceCommunication.GetMediaStatus();
        }

        public void SetDeviceState(DeviceState state, string text = null)
        {
            deviceControl?.SetStatus(state, text);
        }

        //TODO: remove here, move to communication.
        public bool IsConnected()
        {
            var state = deviceCommunication.GetDeviceState();
            return !(state.Equals(DeviceState.NotConnected) ||
                state.Equals(DeviceState.ConnectError) ||
                state.Equals(DeviceState.Closed));
        }

        public void OnVolumeUpdate(Volume volume)
        {
            volumeSetting = volume;
            deviceControl?.OnVolumeUpdate(volume);
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
            deviceCommunication.VolumeSet(volumeSetting);
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

            deviceCommunication.VolumeMute(!volumeSetting.muted);
        }

        public bool Stop()
        {
            return deviceCommunication.Stop();
        }

        public bool AddStreamingConnection(string remoteAddress, Socket socket)
        {
            if (discoveredSsdpDevice.DescriptionLocation.Host.Equals(remoteAddress))
            {
                streamingConnection = DependencyFactory.Container.Resolve<StreamingConnection>();
                streamingConnection.SetSocket(socket);
                streamingConnection.SendStartStreamingResponse();
                return true;
            }

            return false;
        }

        public string GetUsn()
        {
            if (discoveredSsdpDevice != null)
                return discoveredSsdpDevice.Usn;

            return string.Empty;
        }

        public string GetHost()
        {
            if (discoveredSsdpDevice != null)
                return discoveredSsdpDevice.DescriptionLocation.Host;

            return string.Empty;
        }

        public string GetFriendlyName()
        {
            if (ssdpDevice != null)
                return ssdpDevice.FriendlyName;

            return string.Empty;
        }

        public DeviceControl GetDeviceControl()
        {
            return deviceControl;
        }

        public void SetDeviceControl(DeviceControl deviceControlIn)
        {
            deviceControl = deviceControlIn;
        }

        public void SetMenuItem(MenuItem menuItemIn)
        {
            menuItem = menuItemIn;
        }

        public MenuItem GetMenuItem()
        {
            return menuItem;
        }

        public IDeviceConnection GetDeviceConnection()
        {
            throw new NotImplementedException();
        }

    }
}
