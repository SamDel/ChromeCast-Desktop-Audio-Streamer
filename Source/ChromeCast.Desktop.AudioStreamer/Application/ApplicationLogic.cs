using System;
using System.Drawing;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using ChromeCast.Desktop.AudioStreamer.Classes;
using ChromeCast.Desktop.AudioStreamer.Application.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Streaming.Interfaces;
using ChromeCast.Desktop.AudioStreamer.Discover.Interfaces;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using ChromeCast.Desktop.AudioStreamer.Discover;
using System.Threading;
using ChromeCast.Desktop.AudioStreamer.Rest;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class ApplicationLogic : IApplicationLogic, IDisposable
    {
        private IDevices devices;
        private IMainForm mainForm;
        private IConfiguration configuration;
        private IStreamingRequestsListener streamingRequestListener;
        private IDiscoverDevices discoverDevices;
        private IDeviceStatusTimer deviceStatusTimer;
        private NotifyIcon notifyIcon;
        private const int trbLagMaximumValue = 1000;
        private int reduceLagThreshold = trbLagMaximumValue;
        private UserSettings settings = new UserSettings();
        private Mp3Stream Mp3Stream = null;
        private SupportedStreamFormat StreamFormatSelected = SupportedStreamFormat.Mp3_320;
        private string Culture;
        private ILogger logger;
        private Size defaultSize = new Size(850, 550);
        private TasksToCancel taskList;

        private bool AutoRestart { get; set; } = false;

        public ApplicationLogic(IDevices devicesIn, IDiscoverDevices discoverDevicesIn
            , IConfiguration configurationIn
            , IStreamingRequestsListener streamingRequestListenerIn, IDeviceStatusTimer deviceStatusTimerIn
            , ILogger loggerIn)
        {
            devices = devicesIn;
            devices.SetCallback(OnAddDevice);
            discoverDevices = discoverDevicesIn;
            configuration = configurationIn;
            streamingRequestListener = streamingRequestListenerIn;
            deviceStatusTimer = deviceStatusTimerIn;
            logger = loggerIn;
        }

        /// <summary>
        /// Initialize the application.
        /// </summary>
        public void Initialize()
        {
            taskList = new TasksToCancel();
            AddNotifyIcon();
            LoadSettings();
            configuration.Load(ApplyConfiguration, logger);
            ScanForDevices();
            deviceStatusTimer.StartPollingDevice(devices.OnGetStatus);
            var ipAddress = Network.GetIp4Address();
            if (ipAddress == null)
            {
                logger.Log(Properties.Strings.MessageBox_NoIPAddress);
                return;
            }

            StartTask(() => {
                streamingRequestListener.StartListening(ipAddress, OnStreamingRequestConnect, logger);
            });
            StartTask(() => {
                new RestApi().StartListening(ipAddress, RestApiHandler.Process, logger, devices, mainForm);
            });
        }

        /// <summary>
        /// Callback for the StreamingRequestListener, a device has made a new streaming connection.
        /// </summary>
        /// <param name="socketIn">the connected socket</param>
        /// <param name="httpRequestIn">the HTTP headers, including the 'CAST-DEVICE-CAPABILITIES' header</param>
        public void OnStreamingRequestConnect(Socket socketIn, string httpRequestIn)
        {
            if (devices == null)
                return;

            logger.Log(string.Format("Connection added from {0}", socketIn.RemoteEndPoint));
            devices.AddStreamingConnection(socketIn, httpRequestIn, StreamFormatSelected);
        }

        /// <summary>
        /// Callback for the loopback recorder, new audio data is captured.
        /// </summary>
        /// <param name="dataToSendIn">the audio data in wav format</param>
        /// <param name="formatIn">the wav format that's used</param>
        public void OnRecordingDataAvailable(byte[] dataToSendIn, WaveFormat formatIn)
        {
            if (devices == null)
                return;

            if (!StreamFormatSelected.Equals(SupportedStreamFormat.Wav))
            {
                if (Mp3Stream == null)
                {
                    Mp3Stream = new Mp3Stream(formatIn, StreamFormatSelected, logger);
                }
                Mp3Stream.Encode(dataToSendIn.ToArray());
                dataToSendIn = Mp3Stream.Read();
            }
            if (dataToSendIn.Length > 0)
            {
                devices.OnRecordingDataAvailable(dataToSendIn, formatIn, reduceLagThreshold, StreamFormatSelected);
            }
        }

        /// <summary>
        /// Clear the audio data in the mp3 encoder.
        /// </summary>
        public void ClearMp3Buffer()
        {
            Mp3Stream = null;
        }

        /// <summary>
        /// Callback for Devices, a new device is added.
        /// </summary>
        /// <param name="deviceIn">the new device</param>
        public void OnAddDevice(IDevice deviceIn)
        {
            if (deviceIn == null || mainForm == null)
                return;

            try
            {
                var menuItem = new MenuItem
                {
                    Text = deviceIn.GetFriendlyName()
                };
                menuItem.Click += deviceIn.OnClickPlayPause;
                notifyIcon?.ContextMenu?.MenuItems?.Add(notifyIcon.ContextMenu.MenuItems.Count - 1, menuItem);
                deviceIn.SetMenuItem(menuItem);
            }
            catch (Exception ex)
            {
                logger.Log(ex, "ApplicationLogic.OnAddDevice");
            }
            mainForm.AddDevice(deviceIn);
        }

        /// <summary>
        /// Set the dependencies.
        /// </summary>
        /// <param name="mainFormIn">the form</param>
        public void SetDependencies(MainForm mainFormIn)
        {
            mainForm = mainFormIn;
        }

        /// <summary>
        /// The user changed the checkbox to automatically restart devices when closed.
        /// </summary>
        public void OnSetAutoRestart(bool autoRestartIn)
        {
            AutoRestart = autoRestartIn;
        }

        /// <summary>
        /// Automaticaly restart devices y/n.
        /// </summary>
        public bool GetAutoRestart()
        {
            return AutoRestart;
        }

        /// <summary>
        /// The user changed the ip address in the user interface.
        /// Restart streaming using the new ip address.
        /// </summary>
        /// <param name="ipAddressIn">the selected ip address</param>
        public void ChangeIPAddressUsed(IPAddress ipAddressIn)
        {
            if (devices == null || streamingRequestListener == null)
                return;

            logger.Log($"Change IP4 address: {ipAddressIn.ToString()}");
            devices.Stop();
            streamingRequestListener.StopListening();
            ScanForDevices();
            StartTask(() => {
                streamingRequestListener.StartListening(ipAddressIn, OnStreamingRequestConnect, logger);
            });
            var cancellationTokenSource = new CancellationTokenSource();
            StartTask(() =>
            {
                Task.Delay(2500).Wait();

                if (cancellationTokenSource.IsCancellationRequested)
                    return;

                devices.Start();
            }, cancellationTokenSource);
        }

        /// <summary>
        /// The user changed the stream format in the user interface.
        /// Restart streaming in the new format.
        /// </summary>
        /// <param name="formatIn">the chosen format</param>
        public void SetStreamFormat(SupportedStreamFormat formatIn)
        {
            if (devices == null)
                return;

            if (formatIn != StreamFormatSelected)
            {
                logger.Log($"Set stream format to {formatIn.ToString()}");
                StreamFormatSelected = formatIn;
                Mp3Stream = null;

                devices.Stop();
                devices.Start();
            }
        }

        /// <summary>
        /// The user changed the language in the user interface.
        /// </summary>
        /// <param name="cultureIn">the chosen culture</param>
        public void SetCulture(string cultureIn)
        {
            Culture = cultureIn;
        }

        /// <summary>
        /// Search for new devices in the network.
        /// </summary>
        public void ScanForDevices()
        {
            if (devices == null || discoverDevices == null)
                return;

            discoverDevices.Discover(devices.OnDeviceAvailable);
        }

        /// <summary>
        /// Load and apply the settings.
        /// </summary>
        private void LoadSettings()
        {
            if (settings == null || devices == null || mainForm == null)
                return;

            if (!settings.Upgraded ?? true)
            {
                settings.Upgrade();
                settings.Upgraded = true;
            }

            devices.SetSettings(settings);
            mainForm.SetAutoStart(settings.AutoStartDevices ?? false);
            mainForm.SetAutoRestart(settings.AutoRestart ?? false);
            mainForm.SetStartLastUsedDevices(settings.StartLastUsedDevices ?? false);
            mainForm.SetWindowVisibility(settings.ShowWindowOnStart ?? true);
            mainForm.SetKeyboardHooks(settings.UseKeyboardShortCuts ?? false);
            mainForm.SetStreamFormat(settings.StreamFormat ?? SupportedStreamFormat.Mp3_320);
            mainForm.SetCulture(settings.Culture ?? CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
            mainForm.SetLogDeviceCommunication(settings.LogDeviceCommunication ?? false);
            mainForm.SetLagValue(settings.LagControlValue ?? 1000);
            mainForm.SetStartApplicationWhenWindowsStarts(settings.StartApplicationWhenWindowsStarts ?? false);
            mainForm.SetFilterDevices(settings.FilterDevices ?? FilterDevicesEnum.ShowAll);
            if (settings.Size == null || settings.Size.Value.Width < 50|| settings.Size.Value.Height < 50)
                settings.Size = defaultSize;
            mainForm.SetSize(settings.Size.Value);
            mainForm.SetPosition(
                    Math.Min(Math.Max(settings.Left.Value, 0), Screen.PrimaryScreen.Bounds.Width),
                    Math.Min(Math.Max(settings.Top.Value, 0), Screen.PrimaryScreen.Bounds.Height)
                );
            mainForm.SetExtraBufferInSeconds(settings.ExtraBufferInSeconds ?? 0);
            mainForm.SetRecordingDeviceID(settings.RecordingDeviceID ?? null);
            mainForm.SetAutoMute(settings.AutoMute ?? false);
            if (settings.ChromecastDiscoveredDevices != null)
            {
                settings.ChromecastDiscoveredDevices = RemoveOldEntries(settings.ChromecastDiscoveredDevices);
                for (int i = 0; i < settings.ChromecastDiscoveredDevices.Count; i++)
                {
                    StartTask(DeviceInformation.CheckDeviceIsOn(settings.ChromecastDiscoveredDevices[i], devices.OnDeviceAvailable, logger));
                }
            }
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        private void SaveSettings()
        {
            if (settings == null || devices == null || mainForm == null)
                return;

            var discoveredDevices = settings.ChromecastDiscoveredDevices;
            if (discoveredDevices == null)
                discoveredDevices = new List<DiscoveredDevice>();

            // Remove (old) entries of devices without a saved MAC address,
            // and remove (old) entries of groups without a saved ID.
            discoveredDevices = RemoveOldEntries(discoveredDevices);

            foreach (var host in devices.GetHosts())
            {
                if (host.IsGroup)
                {
                    var discoveredDevice = discoveredDevices.Where(x => x.Id == host.Id);
                    if (!discoveredDevice.Any())
                    {
                        discoveredDevices.Add(host);
                    }
                    else
                    {
                        if (host.DeviceState == Communication.DeviceState.ConnectError)
                        {
                            discoveredDevices.Remove(discoveredDevice.First());
                        }
                        else
                        {
                            discoveredDevice.First().Name = host.Name;
                            discoveredDevice.First().IPAddress = host.IPAddress;
                            discoveredDevice.First().Port = host.Port;
                            discoveredDevice.First().DeviceState = host.DeviceState;
                        }
                    }
                }
                else
                {
                    var discoveredDevice = discoveredDevices.Where(x => x.MACAddress == host.MACAddress);
                    if (!discoveredDevice.Any())
                    {
                        discoveredDevices.Add(host);
                    }
                    else
                    {
                        discoveredDevice.First().DeviceState = host.DeviceState;
                    }
                }
            }
            settings.ChromecastDiscoveredDevices = discoveredDevices;
            settings.UseKeyboardShortCuts = mainForm.GetUseKeyboardShortCuts();
            settings.AutoStartDevices = mainForm.GetAutoStartDevices();
            settings.AutoRestart = mainForm.GetAutoRestart();
            settings.StartLastUsedDevices = mainForm.GetStartLastUsedDevices();
            settings.ShowWindowOnStart = mainForm.GetShowWindowOnStart();
            settings.StreamFormat = StreamFormatSelected;
            settings.Culture = Culture;
            settings.LogDeviceCommunication = mainForm.GetLogDeviceCommunication();
            settings.ShowLagControl = mainForm.GetShowLagControl();
            settings.LagControlValue = mainForm.GetLagValue();
            settings.StartApplicationWhenWindowsStarts = mainForm.GetStartApplicationWhenWindowsStarts();
            settings.FilterDevices = mainForm.GetFilterDevices();
            settings.Size = mainForm.GetSize();
            settings.Left = mainForm.GetLeft();
            settings.Top = mainForm.GetTop();
            settings.ExtraBufferInSeconds = mainForm.GetExtraBufferInSeconds();
            settings.RecordingDeviceID = mainForm.GetRecordingDeviceID();
            settings.AutoMute = mainForm.GetAutoMute();

            settings.Save();
        }

        /// <summary>
        /// Remove (old) entries of devices without a saved MAC address,
        /// and remove (old) entries of groups without a saved ID.
        /// </summary>
        private static List<DiscoveredDevice> RemoveOldEntries(List<DiscoveredDevice> discoveredDevices)
        {
            return discoveredDevices.Where(
                            x => (x.IsGroup && !string.IsNullOrEmpty(x.Id)) || (!x.IsGroup && !string.IsNullOrEmpty(x.MACAddress))).ToList();
        }

        /// <summary>
        /// Reset to the deafult setting.
        /// </summary>
        public void ResetSettings()
        {
            if (devices == null || mainForm == null)
                return;

            if (settings == null)
                settings = new UserSettings();

            settings.ChromecastDiscoveredDevices = new List<DiscoveredDevice>();
            settings.UseKeyboardShortCuts = false;
            settings.AutoStartDevices = false;
            settings.StartLastUsedDevices = false;
            settings.ShowWindowOnStart = true;
            settings.AutoRestart = false;
            settings.StreamFormat = SupportedStreamFormat.Mp3_320;
            settings.Culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            settings.LogDeviceCommunication = false;
            settings.ShowLagControl = false;
            settings.LagControlValue = 1000;
            settings.StartApplicationWhenWindowsStarts = false;
            settings.FilterDevices = FilterDevicesEnum.ShowAll;
            settings.Size = defaultSize;
            settings.Left = Screen.PrimaryScreen.Bounds.Width / 2 - settings.Size.Value.Width / 2;
            settings.Top = Screen.PrimaryScreen.Bounds.Height / 2 - settings.Size.Value.Height / 2;
            settings.ExtraBufferInSeconds = 0;
            settings.RecordingDeviceID = null;
            settings.AutoMute = false;
            devices.SetSettings(settings);
            mainForm.SetAutoStart(settings.AutoStartDevices.Value);
            mainForm.SetStartLastUsedDevices(settings.StartLastUsedDevices.Value);
            mainForm.SetAutoRestart(settings.AutoRestart.Value);
            mainForm.SetWindowVisibility(settings.ShowWindowOnStart.Value);
            mainForm.SetKeyboardHooks(settings.UseKeyboardShortCuts.Value);
            mainForm.SetStreamFormat(settings.StreamFormat.Value);
            mainForm.SetCulture(settings.Culture);
            mainForm.SetLogDeviceCommunication(settings.LogDeviceCommunication.Value);
            mainForm.SetLagValue(settings.LagControlValue.Value);
            mainForm.SetStartApplicationWhenWindowsStarts(settings.StartApplicationWhenWindowsStarts.Value);
            mainForm.SetFilterDevices(settings.FilterDevices.Value);
            mainForm.SetSize(settings.Size.Value);
            mainForm.SetExtraBufferInSeconds(settings.ExtraBufferInSeconds.Value);
            mainForm.SetRecordingDeviceID(settings.RecordingDeviceID);
            mainForm.SetAutoMute(settings.AutoMute.Value);
            settings.Save();
            devices?.Dispose();
            if (notifyIcon?.ContextMenu != null)
            {
                for (int i = notifyIcon.ContextMenu.MenuItems.Count - 2; i >= 0; i--)
                {
                    notifyIcon.ContextMenu.MenuItems[i].Dispose();
                }
            }
            ScanForDevices();
        }

        /// <summary>
        /// Get the streaming url.
        /// </summary>
        /// <returns>the url that can be used to open a stream</returns>
        public string GetStreamingUrl()
        {
            if (mainForm == null || streamingRequestListener == null)
                return null;

            mainForm.GetStreamFormat();
            return streamingRequestListener.GetStreamimgUrl();
        }

        /// <summary>
        /// Close the application.
        /// </summary>
        public void CloseApplication()
        {
            SaveSettings();
            Dispose(true);
        }

        public void SetLagThreshold(int lagThresholdIn)
        {
            reduceLagThreshold = lagThresholdIn;
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            devices?.Dispose();
            streamingRequestListener?.StopListening();
            streamingRequestListener?.Dispose();
            Mp3Stream?.Dispose();
            NativeMethods.StopSetWindowsHooks();
            if (notifyIcon != null) notifyIcon.Visible = false;
            notifyIcon?.Dispose();
            mainForm?.Dispose();
            taskList?.Dispose();
        }

        /// <summary>
        /// The devices filter has changed.
        /// </summary>
        /// <param name="value">new filter value</param>
        public void SetFilterDevices(FilterDevicesEnum value)
        {
            if (devices == null)
                return;

            devices.SetFilterDevices(value);
        }

        /// <summary>
        /// Was the device playing when the application was closed for the last time?
        /// </summary>
        /// <returns>true if the device was playing, or false</returns>
        public bool WasPlaying(DiscoveredDevice discoveredDevice)
        {
            if (settings.ChromecastDiscoveredDevices == null)
                return false;

            for (int i = 0; i < settings.ChromecastDiscoveredDevices.Count; i++)
            {
                if (settings.ChromecastDiscoveredDevices[i].Port == discoveredDevice.Port &&
                    settings.ChromecastDiscoveredDevices[i].Name == discoveredDevice.Name)
                {
                    return settings.ChromecastDiscoveredDevices[i].DeviceState == Communication.DeviceState.Playing ||
                        settings.ChromecastDiscoveredDevices[i].DeviceState == Communication.DeviceState.Buffering ||
                        settings.ChromecastDiscoveredDevices[i].DeviceState == Communication.DeviceState.LoadingMedia;
                }
            }

            return false;
        }


        #region private helpers

        /// <summary>
        /// Add an icon to the systray, with a context menu for the devices.
        /// </summary>
        private void AddNotifyIcon()
        {
            try
            {
                var contextMenu = new ContextMenu();
                var menuItem = new MenuItem
                {
                    Index = 0,
                    Text = Properties.Strings.TrayIcon_Close
                };
                menuItem.Click += new EventHandler(CloseApplication);
                contextMenu.MenuItems.AddRange(new MenuItem[] { menuItem });

                notifyIcon = new NotifyIcon();
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
                notifyIcon.Icon = (Icon)resources.GetObject("$this.Icon");
                notifyIcon.Visible = true;
                notifyIcon.Text = Properties.Strings.MainForm_Text;
                notifyIcon.ContextMenu = contextMenu;
                notifyIcon.Click += mainForm.ToggleFormVisibility;
            }
            catch (Exception ex)
            {
                logger.Log(ex, "ApplicationLogic.AddNotifyIcon");
            }
        }

        /// <summary>
        /// Apply the settings in the configuration file.
        /// </summary>
        /// <param name="ipAddressesDevicesIn">
        /// ip addresses & device names
        /// format: 192.168.0.1,DeviceName1;192.168.0.2,DeviceName2
        /// </param>
        private void ApplyConfiguration(string ipAddressesDevicesIn, bool showLagControl)
        {
            try
            {
                mainForm.ShowLagControl(showLagControl);
                if (!string.IsNullOrWhiteSpace(ipAddressesDevicesIn))
                {
                    var ipDevices = ipAddressesDevicesIn.Split(';');
                    foreach (var ipDevice in ipDevices)
                    {
                        var arrDevice = ipDevice.Split(',');
                        devices.OnDeviceAvailable(
                            new DiscoveredDevice
                            {
                                IPAddress = arrDevice[0],
                                Name = arrDevice[1],
                                Port = 8009 // Port = 8009, adding device groups via the config is not possible.
                            });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(ex, "ApplicationLogic.ApplyConfiguration");
            }
        }

        /// <summary>
        /// Callback for the systray icon to close the application.
        /// </summary>
        private void CloseApplication(object sender, EventArgs e)
        {
            CloseApplication();
        }

        /// <summary>
        /// Start an action in a new task.
        /// </summary>
        public void StartTask(Action action, CancellationTokenSource cancellationTokenSource = null)
        {
            taskList.Add(action, cancellationTokenSource);
        }

        #endregion
    }
}
