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
using System.Net.Http;
using System.Globalization;
using ChromeCast.Desktop.AudioStreamer.Discover;

namespace ChromeCast.Desktop.AudioStreamer.Application
{
    public class ApplicationLogic : IApplicationLogic
    {
        private IDevices devices;
        private IMainForm mainForm;
        private IConfiguration configuration;
        private ILoopbackRecorder loopbackRecorder;
        private IStreamingRequestsListener streamingRequestListener;
        private IDiscoverDevices discoverDevices;
        private IDeviceStatusTimer deviceStatusTimer;
        private NotifyIcon notifyIcon;
        private const int trbLagMaximumValue = 1000;
        private int reduceLagThreshold = trbLagMaximumValue;
        private UserSettings settings = new UserSettings();
        private Mp3Stream Mp3Stream = null;
        private SupportedStreamFormat StreamFormatSelected = SupportedStreamFormat.Wav;
        private string Culture;
        private ILogger logger;

        private bool AutoRestart { get; set; } = false;

        public ApplicationLogic(IDevices devicesIn, IDiscoverDevices discoverDevicesIn
            , ILoopbackRecorder loopbackRecorderIn, IConfiguration configurationIn
            , IStreamingRequestsListener streamingRequestListenerIn, IDeviceStatusTimer deviceStatusTimerIn
            , ILogger loggerIn)
        {
            devices = devicesIn;
            devices.SetCallback(OnAddDevice);
            discoverDevices = discoverDevicesIn;
            loopbackRecorder = loopbackRecorderIn;
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
            var ipAddress = Network.GetIp4Address();
            if (ipAddress == null)
            {
                MessageBox.Show(Properties.Strings.MessageBox_NoIPAddress);
                return;
            }

            Task.Run(() => {
                streamingRequestListener.StartListening(ipAddress, OnStreamingRequestConnect);
            });
            AddNotifyIcon();
            LoadSettings();
            configuration.Load(ApplyConfiguration);
            ScanForDevices();
            deviceStatusTimer.StartPollingDevice(devices.OnGetStatus);
            loopbackRecorder.GetDevices(mainForm);
        }

        /// <summary>
        /// Callback for the StreamingRequestListener, a device has made a new streaming connection.
        /// </summary>
        /// <param name="socketIn">the connected socket</param>
        /// <param name="httpRequestIn">the HTTP headers, including the 'CAST-DEVICE-CAPABILITIES' header</param>
        public void OnStreamingRequestConnect(Socket socketIn, string httpRequestIn)
        {
            Console.WriteLine(string.Format("Connection added from {0}", socketIn.RemoteEndPoint));

            loopbackRecorder?.StartRecording(OnRecordingDataAvailable);
            devices?.AddStreamingConnection(socketIn, httpRequestIn);
        }

        /// <summary>
        /// Callback for the loopback recorder, new audio data is captured.
        /// </summary>
        /// <param name="dataToSendIn">the audio data in wav format</param>
        /// <param name="formatIn">the wav format that's used</param>
        public void OnRecordingDataAvailable(byte[] dataToSendIn, WaveFormat formatIn)
        {
            if (!StreamFormatSelected.Equals(SupportedStreamFormat.Wav))
            {
                if (Mp3Stream == null)
                {
                    Mp3Stream = new Mp3Stream(formatIn, StreamFormatSelected);
                }
                Mp3Stream.Encode(dataToSendIn);
                dataToSendIn = Mp3Stream.Read();
            }
            if (dataToSendIn.Length > 0)
            {
                devices?.OnRecordingDataAvailable(dataToSendIn, formatIn, reduceLagThreshold, StreamFormatSelected);
            }
        }

        /// <summary>
        /// Callback for Devices, a new device is added.
        /// </summary>
        /// <param name="deviceIn">the new device</param>
        public void OnAddDevice(IDevice deviceIn)
        {
            if (deviceIn == null)
                return;

            var menuItem = new MenuItem
            {
                Text = deviceIn.GetFriendlyName()
            };
            menuItem.Click += deviceIn.OnClickPlayPause;
            notifyIcon?.ContextMenu?.MenuItems?.Add(notifyIcon.ContextMenu.MenuItems.Count - 1, menuItem);
            deviceIn.SetMenuItem(menuItem);
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
        /// The user changed the recording device in the user interface.
        /// </summary>
        public void RecordingDeviceChanged()
        {
            if (loopbackRecorder == null)
                return;

            loopbackRecorder.StartRecordingDevice();
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
        public async void ChangeIPAddressUsed(IPAddress ipAddressIn)
        {
            if (devices == null || streamingRequestListener == null)
                return;

            devices.Stop();
            streamingRequestListener.StopListening();
            await Task.Run(() => { streamingRequestListener.StartListening(ipAddressIn, OnStreamingRequestConnect); });
            await Task.Delay(2500);
            devices.Start();
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
        private async void LoadSettings()
        {
            if (settings == null || devices == null || mainForm == null)
                return;

            if (!settings.Upgraded ?? true)
            {
                settings.Upgrade();
                settings.Upgraded = true;
            }

            devices.SetAutoStart(settings.AutoStartDevices ?? false);
            mainForm.SetAutoStart(settings.AutoStartDevices ?? false);
            mainForm.SetAutoRestart(settings.AutoRestart ?? false);
            mainForm.SetWindowVisibility(settings.ShowWindowOnStart ?? true);
            mainForm.SetKeyboardHooks(settings.UseKeyboardShortCuts ?? false);
            mainForm.SetStreamFormat(settings.StreamFormat ?? SupportedStreamFormat.Wav);
            mainForm.SetCulture(settings.Culture ?? CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
            mainForm.SetLogDeviceCommunication(settings.LogDeviceCommunication ?? false);
            mainForm.ShowLagControl(settings.ShowLagControl ?? false);
            mainForm.SetLagValue(settings.LagControlValue ?? 1000);
            if (settings.ChromecastDiscoveredDevices != null)
            {
                for (int i = 0; i < settings.ChromecastDiscoveredDevices.Count; i++)
                {
                    try
                    {
                        // Check if the device is on.
                        var http = new HttpClient();
                        var response = await http.GetAsync($"http://{settings.ChromecastDiscoveredDevices[i].IPAddress}:8008/setup/eureka_info?options=detail");
                        if (response?.StatusCode == HttpStatusCode.OK)
                        {
                            devices.OnDeviceAvailable(settings.ChromecastDiscoveredDevices[i]);
                        }
                    }
                    catch (Exception)
                    {
                    }
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
            foreach (var host in devices.GetHosts())
            {
                if (!discoveredDevices.Any(x => x.IPAddress == host.IPAddress && x.Port == host.Port))
                {
                    discoveredDevices.Add(host);
                }
            }
            settings.ChromecastDiscoveredDevices = discoveredDevices;
            settings.UseKeyboardShortCuts = mainForm.GetUseKeyboardShortCuts();
            settings.AutoStartDevices = mainForm.GetAutoStartDevices();
            settings.ShowWindowOnStart = mainForm.GetShowWindowOnStart();
            settings.AutoRestart = mainForm.GetAutoRestart();
            settings.StreamFormat = StreamFormatSelected;
            settings.Culture = Culture;
            settings.LogDeviceCommunication = mainForm.GetLogDeviceCommunication();
            settings.ShowLagControl = mainForm.GetShowLagControl();
            settings.LagControlValue = mainForm.GetLagValue();

            settings.Save();
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
            settings.ShowWindowOnStart = true;
            settings.AutoRestart = false;
            settings.StreamFormat = SupportedStreamFormat.Wav;
            settings.Culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            settings.LogDeviceCommunication = false;
            settings.ShowLagControl = false;
            settings.LagControlValue = 1000;
            devices.SetAutoStart(settings.AutoStartDevices.Value);
            mainForm.SetAutoStart(settings.AutoStartDevices.Value);
            mainForm.SetAutoRestart(settings.AutoRestart.Value);
            mainForm.SetWindowVisibility(settings.ShowWindowOnStart.Value);
            mainForm.SetKeyboardHooks(settings.UseKeyboardShortCuts.Value);
            mainForm.SetStreamFormat(settings.StreamFormat.Value);
            mainForm.SetCulture(settings.Culture);
            mainForm.SetLogDeviceCommunication(settings.LogDeviceCommunication.Value);
            mainForm.ShowLagControl(settings.ShowLagControl.Value);
            mainForm.SetLagValue(settings.LagControlValue.Value);
            settings.Save();
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
            SetWindowsHook.Stop();
            loopbackRecorder?.StopRecording();
            devices?.Dispose();
            streamingRequestListener?.StopListening();
            if (notifyIcon != null) notifyIcon.Visible = false;
            if (mainForm != null) mainForm.Dispose();
        }

        public void SetLagThreshold(int lagThresholdIn)
        {
            reduceLagThreshold = lagThresholdIn;
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
                logger.Log($"AddNotifyIcon: {ex.Message}");
            }
        }

        /// <summary>
        /// Apply the settings in the configuration file.
        /// </summary>
        /// <param name="ipAddressesDevicesIn">
        /// ip addresses & device names
        /// format: 192.168.0.1,DeviceName1;192.168.0.2,DeviceName2
        /// </param>
        private void ApplyConfiguration(string ipAddressesDevicesIn)
        {
            try
            {
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
                logger.Log($"ApplyConfiguration: {ex.Message}");
            }
        }

        /// <summary>
        /// Callback for the systray icon to close the application.
        /// </summary>
        private void CloseApplication(object sender, EventArgs e)
        {
            CloseApplication();
        }

        #endregion
    }
}
