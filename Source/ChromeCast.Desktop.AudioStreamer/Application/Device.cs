using System;
using System.Net.Sockets;
using System.Windows.Forms;
using NAudio.Wave;
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
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    /// <summary>
    /// Device represents a Chromecast device, or a Chromecast group.
    /// </summary>
    public class Device : IDevice
    {
        private readonly IDeviceCommunication deviceCommunication;
        private IStreamingConnection streamingConnection;
        private readonly IDeviceConnection deviceConnection;
        private readonly DiscoveredDevice discoveredDevice;
        private DeviceControl deviceControl;
        private ToolStripMenuItem menuItem;
        private Volume volumeSetting;
        private DateTime latestVolumeChange;
        private float latestVolumeSet;
        private readonly ILogger logger;
        private DateTime lastGetStatus;
        private bool devicePlayedWhenStopped;
        private bool wasPlayingWhenConnectError;
        private DeviceEureka eureka;
        private Action<DeviceEureka> setDeviceInformationCallback;
        private Action<Action, CancellationTokenSource> startTask;
        //private Action<IDevice> stopGroup;
        private Func<IDevice, bool> isGroupStatusBlank;
        private Action<bool> autoMute;
        private DateTime? lastLoadMessageTime;
        private DateTime? addStreamingConnectionTime;

        private bool isDisposed;

        delegate void SetDeviceStateCallback(DeviceState state, string text = null);

        public Device(ILogger loggerIn, IApplicationLogic applicationLogicIn)
        {
            logger = loggerIn;
            deviceConnection = new DeviceConnection(logger);
            deviceCommunication = new DeviceCommunication(applicationLogicIn, logger);
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
            , Action<IDevice> stopGroupIn, Action<Action, CancellationTokenSource> startTaskIn, Func<IDevice, bool> isGroupStatusBlankIn
            , Action<bool> autoMuteIn)
        {
            setDeviceInformationCallback = setDeviceInformationCallbackIn;
            //stopGroup = stopGroupIn;
            startTask = startTaskIn;
            isGroupStatusBlank = isGroupStatusBlankIn;
            autoMute = autoMuteIn;

            if (discoveredDevice == null || deviceCommunication == null || deviceConnection == null ||
                discoveredDeviceIn == null || setDeviceInformationCallbackIn == null || stopGroupIn == null || isDisposed)
                return;

            var ipChanged = discoveredDevice.IPAddress != discoveredDeviceIn.IPAddress;

            // Logging
            if (ipChanged ||
                discoveredDevice.Name != discoveredDeviceIn.Name ||
                discoveredDevice.Port != discoveredDeviceIn.Port ||
                JsonSerializer.Serialize(discoveredDevice.Eureka?.Multizone?.Groups)
                    != JsonSerializer.Serialize(discoveredDeviceIn.Eureka?.Multizone?.Groups)
               )
            {
                logger.Log($"Discovered device: {discoveredDeviceIn?.Name} {discoveredDeviceIn?.IPAddress}:{discoveredDeviceIn?.Port} {JsonSerializer.Serialize(discoveredDeviceIn?.Eureka?.Multizone?.Groups)} {discoveredDeviceIn?.Id}");
            }

            if (discoveredDeviceIn.Headers != null) discoveredDevice.Headers = discoveredDeviceIn.Headers;
            if (discoveredDeviceIn.IPAddress != null) discoveredDevice.IPAddress = discoveredDeviceIn.IPAddress;
            if (discoveredDeviceIn.MACAddress != null) discoveredDevice.MACAddress = discoveredDeviceIn.MACAddress;
            if (discoveredDeviceIn.Id != null) discoveredDevice.Id = discoveredDeviceIn.Id;
            if (discoveredDeviceIn.Name != null) discoveredDevice.Name = discoveredDeviceIn.Name;
            if (discoveredDeviceIn.Port != 0) discoveredDevice.Port = discoveredDeviceIn.Port;
            if (discoveredDeviceIn.Protocol != null) discoveredDevice.Protocol = discoveredDeviceIn.Protocol;
            if (discoveredDeviceIn.Usn != null) discoveredDevice.Usn = discoveredDeviceIn.Usn;
            discoveredDevice.AddedByDeviceInfo = discoveredDeviceIn.AddedByDeviceInfo;
            if (discoveredDeviceIn.Eureka != null) discoveredDevice.Eureka = discoveredDeviceIn.Eureka;
            if (discoveredDeviceIn.Group != null) discoveredDevice.Group = discoveredDeviceIn.Group;

            deviceCommunication.SetCallback(this, deviceConnection.SendMessage, deviceConnection.IsConnected);
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
            SetDeviceName(GetDiscoveredDevice()?.Name);
            eureka = eurekaIn;
            setDeviceInformationCallback?.Invoke(eureka);
        }

        /// <summary>
        /// Play/Pause button is clicked.
        /// </summary>
        public void OnClickPlayStop()
        {
            if (deviceCommunication == null || isDisposed)
                return;

            // Disabled because group information isn't available since a firmware update.
            //stopGroup(this);
            deviceCommunication.OnPlayStop_Click();
            lastGetStatus = DateTime.Now;
            autoMute(deviceCommunication.GetUserMode() == UserMode.Playing);
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
            if (deviceCommunication == null || isDisposed)
                return;

            deviceCommunication.OnStop_Click();
            autoMute(deviceCommunication.GetUserMode() == UserMode.Playing);
        }

        /// <summary>
        /// Load the stream on the device.
        /// </summary>
        public void Start()
        {
            if (devicePlayedWhenStopped || isDisposed)
            {
                ResumePlaying();
            }
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
            if (streamingConnection == null || dataToSend == null || dataToSend.Length == 0 || isDisposed)
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
            if (deviceCommunication == null || isDisposed)
                return;

            DoFirewallCheck();

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
            if (deviceControl == null || deviceControl.IsDisposed || discoveredDevice == null || isDisposed)
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

                DoFirewallCheckSaveTimes(state);

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
            if (deviceCommunication == null || volumeSetting == null || isDisposed)
                return;

            if (DateTime.Now.Ticks - latestVolumeChange.Ticks < 1000)
                return;

            latestVolumeChange = DateTime.Now;

            if (volumeSetting.level > level)
                while (volumeSetting.level > level) volumeSetting.level -= volumeSetting.stepInterval;
            if (volumeSetting.level < level)
                while (volumeSetting.level < level) volumeSetting.level += volumeSetting.stepInterval;
            if (level > 1) { level = 1; volumeSetting.level = level; }
            if (level < 0) { level = 0; volumeSetting.level = level; }

            deviceCommunication.VolumeSet(volumeSetting);
            latestVolumeSet = level;
        }

        /// <summary>
        /// Volume up.
        /// </summary>
        public void VolumeUp()
        {
            if (volumeSetting == null || isDisposed)
                return;

            VolumeSet(volumeSetting.level + 0.05f);
        }

        /// <summary>
        /// Volume down.
        /// </summary>
        public void VolumeDown()
        {
            if (volumeSetting == null || isDisposed)
                return;

            VolumeSet(volumeSetting.level - 0.05f);
        }

        /// <summary>
        /// Volume mute.
        /// </summary>
        public void VolumeMute()
        {
            if (deviceCommunication == null || volumeSetting == null || isDisposed)
                return;

            deviceCommunication.VolumeMute(!volumeSetting.muted);
        }

        /// <summary>
        /// Stop playing on the device.
        /// </summary>
        public void Stop(bool changeUserMode = false)
        {
            if (deviceCommunication == null || isDisposed)
                return;

            switch (GetDeviceState())
            {
                case DeviceState.Playing:
                case DeviceState.LoadingMedia:
                case DeviceState.LoadingMediaCheckFirewall:
                case DeviceState.Buffering:
                case DeviceState.Paused:
                    devicePlayedWhenStopped = GetDeviceState() == DeviceState.Playing;
                    deviceCommunication.Stop(changeUserMode);
                    SetDeviceState(DeviceState.Closed);
                    break;
                default:
                    break;
            }
            autoMute(deviceCommunication.GetUserMode() == UserMode.Playing);
        }

        /// <summary>
        /// Add the streaming connection if it's for this device.
        /// </summary>
        /// <param name="remoteAddress">remote IP address of the streaming connection</param>
        /// <param name="socket">socket of the streaming connection</param>
        /// <returns></returns>
        public bool AddStreamingConnection(string remoteAddress, Socket socket)
        {
            if (discoveredDevice == null || isDisposed)
                return false;

            addStreamingConnectionTime = DateTime.Now;

            //TODO: Is this right for device groups?
            if ((GetDeviceState() == DeviceState.LoadingMedia ||
                GetDeviceState() == DeviceState.LoadingMediaCheckFirewall ||
                GetDeviceState() == DeviceState.Buffering ||
                GetDeviceState() == DeviceState.Idle) &&
                discoveredDevice.IPAddress == remoteAddress)
            {
                streamingConnection = new StreamingConnection();
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
            if (discoveredDevice == null || isDisposed)
                return string.Empty;

            return discoveredDevice.Usn;
        }

        /// <summary>
        /// Get the host of the device.
        /// </summary>
        public string GetHost()
        {
            if (discoveredDevice == null || isDisposed)
                return string.Empty;

            return discoveredDevice.IPAddress;
        }

        /// <summary>
        /// Get the friendly name of the device.
        /// </summary>
        /// <returns></returns>
        public string GetFriendlyName()
        {
            if (discoveredDevice == null || isDisposed)
                return string.Empty;

            return discoveredDevice.Name;
        }

        /// <summary>
        /// Set the friendly name of the device.
        /// </summary>
        private void SetDeviceName(string name)
        {
            if (deviceControl == null || menuItem == null || isDisposed)
                return;

            deviceControl.SetDeviceName(name);
            menuItem.Text = name;
        }

        /// <summary>
        /// Returns the device state.
        /// </summary>
        public DeviceState GetDeviceState()
        {
            if (discoveredDevice == null || isDisposed)
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
            if (deviceControlIn == null || isDisposed)
                return;

            deviceControl = deviceControlIn;
            deviceControl.SetClickCallBack(OnClickPlayStop, OnClickStop);
        }

        /// <summary>
        /// Set the menu item in the systray.
        /// </summary>
        public void SetMenuItem(ToolStripMenuItem menuItemIn)
        {
            menuItem = menuItemIn;
        }

        /// <summary>
        /// Return the menuitem in the systray.
        /// </summary>
        public ToolStripMenuItem GetMenuItem()
        {
            if (isDisposed)
                return null;

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
            if (deviceCommunication == null || isDisposed)
                return;

            deviceCommunication.OnReceiveMessage(castMessage);
        }

        /// <summary>
        /// Get the port of the device.
        /// </summary>
        /// <returns>the port of the device, or 0</returns>
        public int GetPort()
        {
            if (discoveredDevice == null || isDisposed)
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
            if (discoveredDevice == null || isDisposed)
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
            if (deviceControl == null || isDisposed)
                return;

            var tmpLevel = volume.level;
            volumeSetting = volume;
            if (volume.level != latestVolumeSet && latestVolumeSet != 0)
            {
                volume.level = latestVolumeSet;
                if (LevelIsOk(tmpLevel))
                {
                    latestVolumeSet = 0;
                }
            }
            else if (LevelIsOk(tmpLevel))
            {
                latestVolumeSet = 0;
            }

            deviceControl.OnVolumeUpdate(volume);
        }

        private bool LevelIsOk(float level)
        {
            return Math.Abs(level - latestVolumeSet) <= volumeSetting.stepInterval;
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
            else
            {
                SetDeviceName(discoveredDevice.Name);
            }
        }

        /// <summary>
        /// Resume playing.
        /// </summary>
        public void ResumePlaying()
        {
            if (deviceCommunication == null || isDisposed)
                return;

            deviceCommunication.ResumePlaying();
            autoMute(deviceCommunication.GetUserMode() == UserMode.Playing);
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
            isDisposed = true;
            Stop();
            deviceCommunication?.Dispose();
            streamingConnection?.Dispose();
            deviceConnection?.Dispose();
            deviceControl?.Hide();
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

        /// <summary>
        /// Return the usermode.
        /// </summary>
        /// <returns>the usermode</returns>
        public UserMode GetUserMode()
        {
            return deviceCommunication.GetUserMode();
        }

        public int GetVolumeLevel()
        {
            return (int)Math.Round(volumeSetting.level * 100, 0);
        }

        /// <summary>
        /// Do a check if the firewall is closed.
        /// The device should create a streaming connection in 15 seconds after a LOAD message is send.
        /// </summary>
        private void DoFirewallCheck()
        {
            if (lastLoadMessageTime.HasValue)
            {
                if (addStreamingConnectionTime.HasValue)
                {
                    if ((addStreamingConnectionTime.Value - lastLoadMessageTime.Value).TotalSeconds > 15)
                        SetDeviceState(DeviceState.LoadingMediaCheckFirewall);
                }
                else
                {
                    if ((DateTime.Now - lastLoadMessageTime.Value).TotalSeconds > 15)
                        SetDeviceState(DeviceState.LoadingMediaCheckFirewall);
                }
            }
        }

        /// <summary>
        /// Save the time to do the firewall check later.
        /// </summary>
        /// <param name="state"></param>
        private void DoFirewallCheckSaveTimes(DeviceState state)
        {
            if (state == DeviceState.LoadingMedia)
            {
                lastLoadMessageTime = DateTime.Now;
                addStreamingConnectionTime = null;
            }
            else if (state != DeviceState.Idle &&
                state != DeviceState.Buffering)
            {
                lastLoadMessageTime = null;
            }
        }

        public bool IsDisposed()
        {
            return isDisposed;
        }
    }
}
