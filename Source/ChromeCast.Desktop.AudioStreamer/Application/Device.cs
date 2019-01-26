using System;
using System.Net.Sockets;
using System.Windows.Forms;
using NAudio.Wave;
using Microsoft.Practices.Unity;
using ChromeCast.Desktop.AudioStreamer.Communication;
using ChromeCast.Desktop.AudioStreamer.Communication.Classes;
using ChromeCast.Desktop.AudioStreamer.UserControls;
using ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Communication.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Classes;
using ChromeCast.Desktop.AudioStreamer.Streaming;
using ChromeCast.Desktop.AudioStreamer.ProtocolBuffer;
using ChromeCast.Desktop.AudioStreamer.Discover;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;
using Newtonsoft.Json;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class Device : IDevice
    {
        private IDeviceCommunication deviceCommunication;
        private IStreamingConnection streamingConnection;
        private IDeviceConnection deviceConnection;
        private DiscoveredDevice discoveredDevice;
        private DeviceState deviceState;
        private DeviceControl deviceControl;
        private MenuItem menuItem;
        private Volume volumeSetting;
        private DateTime lastVolumeChange;
        private ILogger logger;
        private DateTime lastGetStatus;

        delegate void SetDeviceStateCallback(DeviceState state, string text = null);

        public Device(ILogger loggerIn, IDeviceConnection deviceConnectionIn, IDeviceCommunication deviceCommunicationIn)
        {
            logger = loggerIn;
            deviceConnection = deviceConnectionIn;
            deviceConnection.SetCallback(GetHost, SetDeviceState, OnReceiveMessage);
            deviceCommunication = deviceCommunicationIn;
            deviceCommunication.SetCallback(SetDeviceState, OnVolumeUpdate, deviceConnection.SendMessage, GetDeviceState, IsConnected, deviceConnection.IsConnected, GetHost, GetPort);
            deviceState = DeviceState.NotConnected;
            discoveredDevice = new DiscoveredDevice();
            volumeSetting = new Volume
            {
                controlType = "attenuation",
                level = 0.0f,
                muted = false,
                stepInterval = 0.05f
            };
        }

        public void SetDiscoveredDevices(DiscoveredDevice discoveredDeviceIn)
        {
            logger.Log($"Discovered device: {JsonConvert.SerializeObject(discoveredDeviceIn)}");
            if (discoveredDeviceIn.Headers != null) discoveredDevice.Headers = discoveredDeviceIn.Headers;
            if (discoveredDeviceIn.IPAddress != null) discoveredDevice.IPAddress = discoveredDeviceIn.IPAddress;
            if (discoveredDeviceIn.Name != null) discoveredDevice.Name = discoveredDeviceIn.Name;
            if (discoveredDeviceIn.Port != 0) discoveredDevice.Port = discoveredDeviceIn.Port;
            if (discoveredDeviceIn.Protocol != null) discoveredDevice.Protocol = discoveredDeviceIn.Protocol;
            deviceConnection.SetPort(discoveredDevice.Port);
            OnGetStatus();
        }

        public void OnClickPlayPause()
        {
            deviceCommunication.OnPlayPause_Click();
        }

        private void OnClickStop()
        {
            deviceCommunication.OnStop_Click();
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
                    if (deviceState != DeviceState.NotConnected)
                    {
                        streamingConnection.SendData(dataToSend, format, reduceLagThreshold, streamFormat);
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
            if (deviceState != DeviceState.Disposed && (DateTime.Now - lastGetStatus).TotalSeconds > 2)
            {
                deviceCommunication.GetStatus();
                lastGetStatus = DateTime.Now;
            }
        }

        public void SetDeviceState(DeviceState state, string text = null)
        {
            if (deviceControl == null || deviceControl.IsDisposed)
                return;

            if (deviceControl.InvokeRequired)
            {
                if (!deviceControl.IsDisposed)
                {
                    try
                    {
                        SetDeviceStateCallback callback = new SetDeviceStateCallback(SetDeviceState);
                        deviceControl?.Invoke(callback, new object[] { state, text });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"SetDeviceState: {ex.Message}");
                    }
                }
            }
            else
            {
                if (state == DeviceState.ConnectError && IsGroup())
                {
                    deviceState = DeviceState.Disposed;
                    deviceControl?.Dispose();
                    menuItem?.Dispose();
                }
                else
                {
                    deviceState = state;
                    deviceControl?.SetStatus(state, text);
                    if (deviceControl != null) deviceControl.Visible = true;
                }
            }
        }

        private bool IsGroup()
        {
            return discoveredDevice.Headers.IndexOf("\"md=Google Cast Group\"") >= 0;
        }

        public bool IsConnected()
        {
            return !(deviceState.Equals(DeviceState.NotConnected) ||
                deviceState.Equals(DeviceState.ConnectError) ||
                deviceState.Equals(DeviceState.Closed));
        }

        public void OnVolumeUpdate(Volume volume)
        {
            volumeSetting = volume;
            deviceControl?.OnVolumeUpdate(volume);
        }

        public void VolumeSet(float level)
        {
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
            VolumeSet(volumeSetting.level + 0.05f);
        }

        public void VolumeDown()
        {
            VolumeSet(volumeSetting.level - 0.05f);
        }

        public void VolumeMute()
        {
            deviceCommunication.VolumeMute(!volumeSetting.muted);
        }

        public void Stop()
        {
            deviceCommunication.Stop();
            SetDeviceState(DeviceState.Closed);
        }

        public bool AddStreamingConnection(string remoteAddress, Socket socket)
        {
            if ((deviceState == DeviceState.LoadingMedia || 
                    deviceState == DeviceState.Buffering || 
                    deviceState == DeviceState.Idle) &&
                discoveredDevice.IPAddress.Equals(remoteAddress))
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
            if (discoveredDevice != null)
                return discoveredDevice.Usn;

            return string.Empty;
        }

        public string GetHost()
        {
            if (discoveredDevice != null)
                return discoveredDevice.IPAddress;

            return string.Empty;
        }

        public string GetFriendlyName()
        {
            if (discoveredDevice != null)
                return discoveredDevice.Name;

            return string.Empty;
        }

        public DeviceState GetDeviceState()
        {
            return deviceState;
        }

        public DeviceControl GetDeviceControl()
        {
            return deviceControl;
        }

        public void SetDeviceControl(DeviceControl deviceControlIn)
        {
            deviceControl = deviceControlIn;
            deviceControl.SetClickCallBack(OnClickPlayPause, OnClickStop);
            if (deviceControl != null)
                deviceControl.Visible = !IsGroup();
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
            return deviceConnection;
        }

        public void OnReceiveMessage(CastMessage castMessage)
        {
            deviceCommunication?.OnReceiveMessage(castMessage);
        }

        public void SetDeviceName(string name)
        {
            deviceControl?.SetDeviceName(name);
            menuItem.Text = name;
        }

        public ushort GetPort()
        {
            return discoveredDevice.Port;
        }

        public DiscoveredDevice GetDiscoveredDevice()
        {
            return discoveredDevice;
        }
    }
}
