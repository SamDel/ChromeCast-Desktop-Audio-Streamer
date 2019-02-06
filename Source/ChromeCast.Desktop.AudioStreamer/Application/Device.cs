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
using System.Threading.Tasks;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    /// <summary>
    /// Device represents a Chromecast device, or a Chromecast group.
    /// </summary>
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
        private bool devicePlayedWhenStopped;
        private bool wasPlayingWhenConnectError;
        private DeviceEureka eureka;

        delegate void SetDeviceStateCallback(DeviceState state, string text = null);

        public Device(ILogger loggerIn, IDeviceConnection deviceConnectionIn, IDeviceCommunication deviceCommunicationIn)
        {
            logger = loggerIn;
            deviceConnection = deviceConnectionIn;
            deviceCommunication = deviceCommunicationIn;
            deviceConnection.SetCallback(GetHost, SetDeviceState, OnReceiveMessage);
            deviceState = DeviceState.NotConnected;
            discoveredDevice = new DiscoveredDevice();
        }

        /// <summary>
        /// Initialize a device.
        /// </summary>
        /// <param name="discoveredDeviceIn">the discovered device</param>
        public void Initialize(DiscoveredDevice discoveredDeviceIn)
        {
            if (discoveredDevice == null || deviceCommunication == null || deviceConnection == null)
                return;

            logger.Log($"Discovered device: {JsonConvert.SerializeObject(discoveredDeviceIn)}");
            if (discoveredDeviceIn.Headers != null) discoveredDevice.Headers = discoveredDeviceIn.Headers;
            if (discoveredDeviceIn.IPAddress != null) discoveredDevice.IPAddress = discoveredDeviceIn.IPAddress;
            if (discoveredDeviceIn.Name != null) discoveredDevice.Name = discoveredDeviceIn.Name;
            if (discoveredDeviceIn.Port != 0) discoveredDevice.Port = discoveredDeviceIn.Port;
            if (discoveredDeviceIn.Protocol != null) discoveredDevice.Protocol = discoveredDeviceIn.Protocol;
            deviceCommunication.SetCallback(SetDeviceState,
                OnVolumeUpdate,
                deviceConnection.SendMessage,
                GetDeviceState,
                IsConnected,
                deviceConnection.IsConnected,
                GetHost,
                GetPort,
                SendSilence,
                WasPlayingWhenStopped);
            deviceConnection.SetPort(discoveredDevice.Port);
            OnGetStatus();
            DeviceInformation.GetDeviceInformation(discoveredDevice, SetDeviceInformation);
            volumeSetting = new Volume
            {
                controlType = "attenuation",
                level = 0.0f,
                muted = false,
                stepInterval = 0.05f
            };
        }

        /// <summary>
        /// Set the device information.
        /// </summary>
        /// <param name="eureka"></param>
        private void SetDeviceInformation(DeviceEureka eurekaIn)
        {
            SetDeviceName(eurekaIn.Name);
            eureka = eurekaIn;
        }

        /// <summary>
        /// Play/Pause button is clicked.
        /// </summary>
        public void OnClickPlayStop()
        {
            if (deviceCommunication == null)
                return;

            deviceCommunication.OnPlayStop_Click();
        }

        public void OnClickPlayPause(object sender, EventArgs e)
        {
            OnClickPlayStop();
        }

        /// <summary>
        /// Stop button is clicked.
        /// </summary>
        private void OnClickStop()
        {
            if (deviceCommunication == null)
                return;

            deviceCommunication.OnStop_Click();
        }

        /// <summary>
        /// Load the stream on the device.
        /// </summary>
        public void Start()
        {
            if (deviceCommunication == null)
                return;

            if (devicePlayedWhenStopped)
                deviceCommunication.LoadMedia();
        }

        /// <summary>
        /// Stream the recorded data to the device.
        /// </summary>
        /// <param name="dataToSend">tha audio data</param>
        /// <param name="format">the wav format</param>
        /// <param name="reduceLagThreshold">lag value</param>
        /// <param name="streamFormat">the stream format</param>
        public void OnRecordingDataAvailable(byte[] dataToSend, WaveFormat format, int reduceLagThreshold, SupportedStreamFormat streamFormat)
        {
            if (streamingConnection == null || dataToSend == null || dataToSend.Length == 0)
                return;

            if (streamingConnection.IsConnected())
            {
                if (deviceState != DeviceState.NotConnected &&
                    deviceState != DeviceState.Paused) // When you keep streaming to a device when it is paused, the application stops streaming after a while (local buffers full?)
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

        /// <summary>
        /// Get the device status.
        /// </summary>
        public void OnGetStatus()
        {
            if (deviceCommunication == null)
                return;

            if (deviceState != DeviceState.Disposed && (DateTime.Now - lastGetStatus).TotalSeconds > 2)
            {
                deviceCommunication.GetStatus();
                lastGetStatus = DateTime.Now;
            }
        }

        /// <summary>
        /// Set the device status.
        /// </summary>
        /// <param name="state">the state</param>
        /// <param name="statusText">status text</param>
        public void SetDeviceState(DeviceState state, string statusText = null)
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
                        deviceControl?.Invoke(callback, new object[] { state, statusText });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"SetDeviceState: {ex.Message}");
                    }
                }
            }
            else
            {
                // Restart when recovering from a connection error.
                if (state != DeviceState.ConnectError && wasPlayingWhenConnectError)
                {
                    wasPlayingWhenConnectError = false;
                    if (state != DeviceState.Playing)
                    {
                        Task.Run(() => {
                            Task.Delay(5000).Wait();
                            OnClickPlayStop();
                        });
                    }
                }
                else if (state == DeviceState.ConnectError && deviceState == DeviceState.Playing)
                {
                    wasPlayingWhenConnectError = true;
                }

                if (state == DeviceState.ConnectError && IsGroup())
                {
                    deviceState = DeviceState.Disposed;
                    deviceControl?.Dispose();
                    menuItem?.Dispose();
                }
                else
                {
                    deviceState = state;
                    deviceControl?.SetStatus(state, statusText);
                    if (deviceControl != null) deviceControl.Visible = true;
                }
            }
        }

        /// <summary>
        /// Set the volume on the device
        /// </summary>
        /// <param name="level"></param>
        public void VolumeSet(float level)
        {
            if (deviceCommunication == null || volumeSetting == null)
                return;

            if (DateTime.Now.Ticks - lastVolumeChange.Ticks < 1000)
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

        /// <summary>
        /// Volume up.
        /// </summary>
        public void VolumeUp()
        {
            if (volumeSetting == null)
                return;

            VolumeSet(volumeSetting.level + 0.05f);
        }

        /// <summary>
        /// Volume down.
        /// </summary>
        public void VolumeDown()
        {
            if (volumeSetting == null)
                return;

            VolumeSet(volumeSetting.level - 0.05f);
        }

        /// <summary>
        /// Volume mute.
        /// </summary>
        public void VolumeMute()
        {
            if (deviceCommunication == null || volumeSetting == null)
                return;

            deviceCommunication.VolumeMute(!volumeSetting.muted);
        }

        /// <summary>
        /// Stop playing on the device.
        /// </summary>
        public void Stop()
        {
            if (deviceCommunication == null)
                return;

            switch (deviceState)
            {
                case DeviceState.Playing:
                case DeviceState.LoadingMedia:
                case DeviceState.Buffering:
                case DeviceState.Paused:
                    devicePlayedWhenStopped = deviceState == DeviceState.Playing;
                    deviceCommunication.Stop();
                    SetDeviceState(DeviceState.Closed);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Add the streaming connection if it's for this device.
        /// </summary>
        /// <param name="remoteAddress">remote IP address of the streaming connection</param>
        /// <param name="socket">socket of the streaming connection</param>
        /// <returns></returns>
        public bool AddStreamingConnection(string remoteAddress, Socket socket)
        {
            if (discoveredDevice == null)
                return false;

            //TODO: Is this right for device groups?
            if ((deviceState == DeviceState.LoadingMedia || 
                deviceState == DeviceState.Buffering || 
                deviceState == DeviceState.Idle) &&
                discoveredDevice.IPAddress == remoteAddress)
            {
                streamingConnection = DependencyFactory.Container.Resolve<StreamingConnection>();
                streamingConnection.SetSocket(socket);
                streamingConnection.SendStartStreamingResponse();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get the Usn of the device.
        /// </summary>
        public string GetUsn()
        {
            if (discoveredDevice == null)
                return string.Empty;

            return discoveredDevice.Usn;
        }

        /// <summary>
        /// Get the host of the device.
        /// </summary>
        public string GetHost()
        {
            if (discoveredDevice == null)
                return string.Empty;

            return discoveredDevice.IPAddress;
        }

        /// <summary>
        /// Get the friendly name of the device.
        /// </summary>
        /// <returns></returns>
        public string GetFriendlyName()
        {
            if (discoveredDevice == null)
                return string.Empty;

            return discoveredDevice.Name;
        }

        /// <summary>
        /// Set the friendly name of the device.
        /// </summary>
        private void SetDeviceName(string name)
        {
            if (deviceControl == null || menuItem == null)
                return;

            deviceControl.SetDeviceName(name);
            menuItem.Text = name;
        }

        /// <summary>
        /// Returns the device state.
        /// </summary>
        public DeviceState GetDeviceState()
        {
            return deviceState;
        }

        /// <summary>
        /// Returns the user interface control of the device.
        /// </summary>
        public DeviceControl GetDeviceControl()
        {
            return deviceControl;
        }

        /// <summary>
        /// Set the user interface control of the device.
        /// </summary>
        public void SetDeviceControl(DeviceControl deviceControlIn)
        {
            if (deviceControlIn == null)
                return;

            deviceControl = deviceControlIn;
            deviceControl.SetClickCallBack(OnClickPlayStop, OnClickStop);
            deviceControl.Visible = !IsGroup();
        }

        /// <summary>
        /// Set the menu item in the systray.
        /// </summary>
        public void SetMenuItem(MenuItem menuItemIn)
        {
            menuItem = menuItemIn;
        }

        /// <summary>
        /// Return the menuitem in the systray.
        /// </summary>
        public MenuItem GetMenuItem()
        {
            return menuItem;
        }

        /// <summary>
        /// Returns the connection for the control messages.
        /// </summary>
        public IDeviceConnection GetDeviceConnection()
        {
            return deviceConnection;
        }

        /// <summary>
        /// A message from the device is received.
        /// </summary>
        public void OnReceiveMessage(CastMessage castMessage)
        {
            if (deviceCommunication == null)
                return;

            deviceCommunication.OnReceiveMessage(castMessage);
        }

        /// <summary>
        /// Get the port of the device.
        /// </summary>
        /// <returns>the port of the device, or 0</returns>
        public ushort GetPort()
        {
            if (discoveredDevice == null)
                return 0;

            return discoveredDevice.Port;
        }

        /// <summary>
        /// Return the discovered device.
        /// </summary>
        public DiscoveredDevice GetDiscoveredDevice()
        {
            return discoveredDevice;
        }

        #region private helpers

        /// <summary>
        /// Determine if this is a Chromecast group.
        /// </summary>
        /// <returns>true if it's a group, false if it's not a group</returns>
        public bool IsGroup()
        {
            return discoveredDevice?.Headers?.IndexOf("\"md=Google Cast Group\"") >= 0;
        }

        /// <summary>
        /// Determine if the device is in a connected state.
        /// </summary>
        /// <returns>true if it's connected, or false if not</returns>
        private bool IsConnected()
        {
            return !(deviceState.Equals(DeviceState.NotConnected) ||
                deviceState.Equals(DeviceState.ConnectError) ||
                deviceState.Equals(DeviceState.Closed));
        }

        /// <summary>
        /// A volume update from the device.
        /// </summary>
        /// <param name="volume">the volume on the device</param>
        private void OnVolumeUpdate(Volume volume)
        {
            if (deviceControl == null)
                return;

            volumeSetting = volume;
            deviceControl.OnVolumeUpdate(volume);
        }

        /// <summary>
        /// Send silence to the device.
        /// </summary>
        public void SendSilence()
        {
            OnRecordingDataAvailable(Properties.Resources.silence,
                new WaveFormat(44100, 2), 1000, SupportedStreamFormat.Mp3_320);
        }

        /// <summary>
        /// Return if the device was playing when stopped. Used for auto restart.
        /// </summary>
        /// <returns></returns>
        private bool WasPlayingWhenStopped()
        {
            var returnValue = devicePlayedWhenStopped;
            devicePlayedWhenStopped = false;
            return returnValue;
        }

        #endregion
    }
}
