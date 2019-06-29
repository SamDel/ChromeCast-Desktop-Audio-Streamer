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
using System.Threading;

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
        private DeviceControl deviceControl;
        private MenuItem menuItem;
        private Volume volumeSetting;
        private DateTime lastVolumeChange;
        private ILogger logger;
        private DateTime lastGetStatus;
        private bool devicePlayedWhenStopped;
        private bool wasPlayingWhenConnectError;
        private DeviceEureka eureka;
        private Action<DeviceEureka> setDeviceInformationCallback;
        private Action<Action, CancellationTokenSource> startTask;
        private Action<IDevice> stopGroup;
        private Func<IDevice, bool> isGroupStatusBlank;

        delegate void SetDeviceStateCallback(DeviceState state, string text = null);

        public Device(ILogger loggerIn, IDeviceConnection deviceConnectionIn, IDeviceCommunication deviceCommunicationIn)
        {
            logger = loggerIn;
            deviceConnection = deviceConnectionIn;
            deviceCommunication = deviceCommunicationIn;
            deviceConnection.SetCallback(GetHost, GetPort, SetDeviceState, OnReceiveMessage, StartTask);
            discoveredDevice = new DiscoveredDevice
            {
                DeviceState = DeviceState.NotConnected
            };
            volumeSetting = new Volume
            {
                controlType = "attenuation",
                level = 0.0f,
                muted = false,
                stepInterval = 0.05f
            };
        }

        /// <summary>
        /// Initialize a device.
        /// </summary>
        /// <param name="discoveredDeviceIn">the discovered device</param>
        public void Initialize(DiscoveredDevice discoveredDeviceIn, Action<DeviceEureka> setDeviceInformationCallbackIn
            , Action<IDevice> stopGroupIn, Action<Action, CancellationTokenSource> startTaskIn, Func<IDevice, bool> isGroupStatusBlankIn)
        {
            setDeviceInformationCallback = setDeviceInformationCallbackIn;
            stopGroup = stopGroupIn;
            startTask = startTaskIn;
            isGroupStatusBlank = isGroupStatusBlankIn;

            if (discoveredDevice == null || deviceCommunication == null || deviceConnection == null ||
                discoveredDeviceIn == null || setDeviceInformationCallbackIn == null || stopGroupIn == null)
                return;

            var ipChanged = discoveredDevice.IPAddress != discoveredDeviceIn.IPAddress;

            // Logging
            if (ipChanged ||
                discoveredDevice.Name != discoveredDeviceIn.Name ||
                discoveredDevice.Port != discoveredDeviceIn.Port ||
                JsonConvert.SerializeObject(discoveredDevice.Eureka?.Multizone?.Groups)
                    != JsonConvert.SerializeObject(discoveredDeviceIn.Eureka?.Multizone?.Groups))
            {
                logger.Log($"Discovered device: {discoveredDeviceIn?.Name} {discoveredDeviceIn?.IPAddress}:{discoveredDeviceIn?.Port} {JsonConvert.SerializeObject(discoveredDeviceIn?.Eureka?.Multizone?.Groups)}");
            }

            if (discoveredDeviceIn.Headers != null) discoveredDevice.Headers = discoveredDeviceIn.Headers;
            if (discoveredDeviceIn.IPAddress != null) discoveredDevice.IPAddress = discoveredDeviceIn.IPAddress;
            if (discoveredDeviceIn.Name != null) discoveredDevice.Name = discoveredDeviceIn.Name;
            if (discoveredDeviceIn.Port != 0) discoveredDevice.Port = discoveredDeviceIn.Port;
            if (discoveredDeviceIn.Protocol != null) discoveredDevice.Protocol = discoveredDeviceIn.Protocol;
            if (discoveredDeviceIn.Usn != null) discoveredDevice.Usn = discoveredDeviceIn.Usn;
            discoveredDevice.AddedByDeviceInfo = discoveredDeviceIn.AddedByDeviceInfo;
            if (discoveredDeviceIn.Eureka != null) discoveredDevice.Eureka = discoveredDeviceIn.Eureka;
            if (discoveredDeviceIn.Group != null) discoveredDevice.Group = discoveredDeviceIn.Group;

            deviceCommunication.SetCallback(this, deviceConnection.SendMessage, deviceConnection.IsConnected);
            if (!IsGroup() || (IsGroup() && discoveredDevice.AddedByDeviceInfo))
                OnGetStatus();
            if (ipChanged && GetDeviceState() == DeviceState.Playing)
            {
                ResumePlaying();
            }
        }

        /// <summary>
        /// Set the device information.
        /// </summary>
        /// <param name="eureka"></param>
        private void SetDeviceInformation(DeviceEureka eurekaIn)
        {
            SetDeviceName(eurekaIn.GetName());
            eureka = eurekaIn;
            setDeviceInformationCallback?.Invoke(eureka);
        }

        /// <summary>
        /// Play/Pause button is clicked.
        /// </summary>
        public void OnClickPlayStop()
        {
            if (deviceCommunication == null)
                return;

            stopGroup(this);
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
            if (devicePlayedWhenStopped)
                ResumePlaying();
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
                if (GetDeviceState() != DeviceState.NotConnected &&
                    GetDeviceState() != DeviceState.Paused) // When you keep streaming to a device when it is paused, the application stops streaming after a while (local buffers full?)
                {
                    streamingConnection.SendData(dataToSend, format, reduceLagThreshold, streamFormat);
                }
            }
            else
            {
                logger.Log($"Connection closed from {streamingConnection.GetRemoteEndPoint()}");
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

            if (GetDeviceState() != DeviceState.Disposed && (DateTime.Now - lastGetStatus).TotalSeconds > 5)
            {
                deviceCommunication.GetStatus();
                GetDeviceInformation();
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
            if (deviceControl == null || deviceControl.IsDisposed || discoveredDevice == null)
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
                        logger.Log(ex, "Device.SetDeviceState");
                    }
                }
            }
            else
            {
                // Restart when recovering from a connection error.
                if (state != DeviceState.ConnectError && wasPlayingWhenConnectError)
                {
                    wasPlayingWhenConnectError = false;
                    ResumePlaying();
                }
                else if (state == DeviceState.ConnectError && GetDeviceState() == DeviceState.Playing)
                {
                    wasPlayingWhenConnectError = true;
                }

                discoveredDevice.DeviceState = state;
                deviceControl?.SetStatus(discoveredDevice.DeviceState, statusText);
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
        public void Stop(bool changeUserMode = false)
        {
            if (deviceCommunication == null)
                return;

            switch (GetDeviceState())
            {
                case DeviceState.Playing:
                case DeviceState.LoadingMedia:
                case DeviceState.Buffering:
                case DeviceState.Paused:
                    devicePlayedWhenStopped = GetDeviceState() == DeviceState.Playing;
                    deviceCommunication.Stop(changeUserMode);
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
            if ((GetDeviceState() == DeviceState.LoadingMedia ||
                GetDeviceState() == DeviceState.Buffering ||
                GetDeviceState() == DeviceState.Idle) &&
                discoveredDevice.IPAddress == remoteAddress)
            {
                streamingConnection = DependencyFactory.Container.Resolve<StreamingConnection>();
                streamingConnection.SetDependencies(socket, this, logger);
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
            if (discoveredDevice == null)
                return DeviceState.Disposed;

            return discoveredDevice.DeviceState;
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
        public int GetPort()
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

        /// <summary>
        /// Determine if this is a Chromecast group.
        /// </summary>
        /// <returns>true if it's a group, false if it's not a group</returns>
        public bool IsGroup()
        {
            if (discoveredDevice == null)
                return false;

            return discoveredDevice.IsGroup;
        }

        /// <summary>
        /// Determine if the device is in a connected state.
        /// </summary>
        /// <returns>true if it's connected, or false if not</returns>
        public bool IsConnected()
        {
            return !(GetDeviceState().Equals(DeviceState.NotConnected) ||
                GetDeviceState().Equals(DeviceState.ConnectError) ||
                GetDeviceState().Equals(DeviceState.Closed));
        }

        /// <summary>
        /// A volume update from the device.
        /// </summary>
        /// <param name="volume">the volume on the device</param>
        public void OnVolumeUpdate(Volume volume)
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
            var silence = new WavGenerator().GetSilenceBytes(5);
            OnRecordingDataAvailable(silence, new WaveFormat(44100, 2), 1000, SupportedStreamFormat.Mp3_320);
        }

        /// <summary>
        /// Get the information of the device.
        /// </summary>
        private void GetDeviceInformation()
        {
            if (!IsGroup())
            {
                startTask(DeviceInformation.GetDeviceInformation(discoveredDevice, SetDeviceInformation, logger), null);
            }
        }

        /// <summary>
        /// Resume playing.
        /// </summary>
        public void ResumePlaying()
        {
            if (deviceCommunication == null)
                return;

            deviceCommunication.ResumePlaying();
        }

        /// <summary>
        /// Return device eureka information.
        /// </summary>
        public DeviceEureka GetEureka()
        {
            return eureka;
        }

        /// <summary>
        /// Start a task
        /// </summary>
        public void StartTask(Action action)
        {
            if (startTask == null)
                Task.Run(action);
            else
                startTask(action, null);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            deviceCommunication?.Dispose();
            streamingConnection?.Dispose();
            deviceConnection?.Dispose();
            deviceControl?.Dispose();
        }

        /// <summary>
        /// Check if the status text of the device is empty,
        /// For groups the status text of all devices in the group should be empty.
        /// </summary>
        /// <returns>true if the status text(s) are empty, or false</returns>
        public bool IsStatusTextBlank()
        {
            if (IsGroup())
            {
                return isGroupStatusBlank(this);
            }
            else
            {
                var statusText = GetStatusText();
                return IsStatusTextBlankCheck(statusText);
            }
        }

        /// <summary>
        /// Get the status text returned by the device.
        /// </summary>
        /// <returns>the status text</returns>
        public string GetStatusText()
        {
            return deviceCommunication.GetStatusText();
        }

        /// <summary>
        /// Check if the status text of the device is blank.
        /// </summary>
        public bool IsStatusTextBlankCheck(string statusText)
        {
            return string.IsNullOrEmpty(statusText) || statusText?.IndexOf(Properties.Strings.ChromeCast_StreamTitle) >= 0;
        }
    }
}
