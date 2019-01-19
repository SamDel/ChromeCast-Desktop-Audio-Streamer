using System;
using System.Drawing;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rssdp;
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
        private string streamingUrl = string.Empty;
        private bool playingOnIpOrFormatChange;
        private UserSettings settings = new UserSettings();
        private Mp3Stream Mp3Stream = null;
        private SupportedStreamFormat StreamFormatSelected = SupportedStreamFormat.Wav;
        private string Culture;

        private bool AutoRestart { get; set; } = false;

        public ApplicationLogic(IDevices devicesIn, IDiscoverDevices discoverDevicesIn
            , ILoopbackRecorder loopbackRecorderIn, IConfiguration configurationIn
            , IStreamingRequestsListener streamingRequestListenerIn, IDeviceStatusTimer deviceStatusTimerIn)
        {
            devices = devicesIn;
            devices.SetCallback(OnAddDevice);
            discoverDevices = discoverDevicesIn;
            loopbackRecorder = loopbackRecorderIn;
            configuration = configurationIn;
            streamingRequestListener = streamingRequestListenerIn;
            deviceStatusTimer = deviceStatusTimerIn;
        }

        public void Start()
        {
            var ipAddress = Network.GetIp4Address();
            Task.Run(() => { streamingRequestListener.StartListening(ipAddress, OnStreamingRequestsListen, OnStreamingRequestConnect); });
            AddNotifyIcon();
            LoadSettings();
            configuration.Load(SetConfiguration);
            ScanForDevices();
            deviceStatusTimer.StartPollingDevice(devices.OnGetStatus);
            loopbackRecorder.GetDevices(mainForm);
        }

        private void ToggleFormVisibility(object sender, EventArgs e)
        {
            if (e.GetType().Equals(typeof(MouseEventArgs)))
            {
                if (((MouseEventArgs)e).Button != MouseButtons.Left) return;
            }

            mainForm.ToggleVisibility();
        }

        public void OnStreamingRequestsListen(string host, int port)
        {
            Console.WriteLine(string.Format("Streaming from {0}:{1}", host, port));
            streamingUrl = string.Format("http://{0}:{1}/", host, port);
        }

        public void OnStreamingRequestConnect(Socket socket, string httpRequest)
        {
            Console.WriteLine(string.Format("Connection added from {0}", socket.RemoteEndPoint));

            loopbackRecorder.StartRecording(OnRecordingDataAvailable);
            devices.AddStreamingConnection(socket, httpRequest);
        }

        public void OnRecordingDataAvailable(byte[] dataToSend, WaveFormat format)
        {
            if (!StreamFormatSelected.Equals(SupportedStreamFormat.Wav))
            {
                if (Mp3Stream == null)
                {
                    Mp3Stream = new Mp3Stream(format, StreamFormatSelected);
                }
                Mp3Stream.Encode(dataToSend);
                dataToSend = Mp3Stream.Read();
            }
            if (dataToSend.Length > 0)
            {
                devices.OnRecordingDataAvailable(dataToSend, format, reduceLagThreshold, StreamFormatSelected);
            }
        }

        public void OnSetHooks(bool setHooks)
        {
            if (setHooks)
                SetWindowsHook.Start(devices);
            else
                SetWindowsHook.Stop();
        }

        public void OnAddDevice(IDevice device)
        {
            var menuItem = new MenuItem
            {
                Text = device.GetFriendlyName()
            };
            notifyIcon.ContextMenu.MenuItems.Add(notifyIcon.ContextMenu.MenuItems.Count - 1, menuItem);
            device.SetMenuItem(menuItem);

            mainForm.AddDevice(device);
            device.OnGetStatus();
        }

        private void AddNotifyIcon()
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
            notifyIcon.Icon = ((Icon)(resources.GetObject("$this.Icon")));
            notifyIcon.Visible = true;
            notifyIcon.Text = Properties.Strings.MainForm_Text;
            notifyIcon.ContextMenu = contextMenu;
            notifyIcon.Click += ToggleFormVisibility;
        }

        public string GetStreamingUrl()
        {
            mainForm.GetStreamFormat();
            return streamingUrl;
        }

        public void SetLagThreshold(int lagThreshold)
        {
            reduceLagThreshold = lagThreshold;
        }

        public void SetConfiguration(bool showLog, bool showLagControl, int lagValue, string ipAddressesDevices)
        {
            if (!string.IsNullOrWhiteSpace(ipAddressesDevices))
            {
                var ipDevices = ipAddressesDevices.Split(';');
                foreach (var ipDevice in ipDevices)
                {
                    var arrDevice = ipDevice.Split(',');
                    devices.OnDeviceAvailable(
                            new DiscoveredSsdpDevice { DescriptionLocation = new Uri($"http://{arrDevice[0]}") },
                            new SsdpRootDevice { FriendlyName = arrDevice[1] }
                        );
                }
            }
        }

        public void CloseApplication()
        {
            SaveSettings();
            SetWindowsHook.Stop();
            loopbackRecorder.StopRecording();
            devices.Dispose();
            streamingRequestListener.StopListening();
            notifyIcon.Visible = false;
            mainForm.Dispose();
        }

        private void CloseApplication(object sender, EventArgs e)
        {
            CloseApplication();
        }

        public void SetDependencies(MainForm mainFormIn)
        {
            mainForm = mainFormIn;
        }

        public void RecordingDeviceChanged()
        {
            loopbackRecorder.StartRecordingDevice();
        }

        public void OnSetAutoRestart(bool autoRestart)
        {
            AutoRestart = autoRestart;
        }

        public bool GetAutoRestart()
        {
            if (playingOnIpOrFormatChange)
            {
                playingOnIpOrFormatChange = false;
                return true;
            }
            else
            {
                return AutoRestart;
            }
        }

        public async void ChangeIPAddressUsed(IPAddress ipAddress)
        {
            playingOnIpOrFormatChange = devices.Stop();
            streamingRequestListener.StopListening();
            await Task.Run(() => { streamingRequestListener.StartListening(ipAddress, OnStreamingRequestsListen, OnStreamingRequestConnect); });
            if (playingOnIpOrFormatChange)
            {
                await Task.Delay(2500);
                devices.Start();
                await Task.Delay(15000);
                playingOnIpOrFormatChange = false;
            }
        }

        public void ScanForDevices()
        {
            discoverDevices.Discover(devices.OnDeviceAvailable);
        }

        private async void LoadSettings()
        {
            if (!settings.Upgraded ?? true)
            {
                settings.Upgrade();
                settings.Upgraded = true;
            }

            devices.SetAutoStart(settings.AutoStartDevices ?? false);
            mainForm.SetAutoStart(settings.AutoStartDevices ?? false);
            mainForm.SetAutoRestart(settings.AutoRestart ?? false);
            mainForm.SetWindowVisibility(settings.ShowWindowOnStart ?? true);
            OnSetHooks(settings.UseKeyboardShortCuts ?? false);
            mainForm.SetKeyboardHooks(settings.UseKeyboardShortCuts ?? false);
            mainForm.SetStreamFormat(settings.StreamFormat ?? SupportedStreamFormat.Wav);
            mainForm.SetCulture(settings.Culture ?? CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
            mainForm.SetLogDeviceCommunication(settings.LogDeviceCommunication ?? false);
            mainForm.ShowLagControl(settings.ShowLagControl ?? false);
            mainForm.SetLagValue(settings.LagControlValue ?? 1000);
            if (settings.ChromecastHosts != null)
            {
                for (int i = 0; i < settings.ChromecastHosts.Count; i++)
                {
                    try
                    {
                        // Check if the device is on.
                        var http = new HttpClient();
                        var response = await http.GetAsync($"http://{settings.ChromecastHosts[i].Ip}:8008/setup/eureka_info?options=detail");
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            devices.OnDeviceAvailable(
                                    new DiscoveredSsdpDevice { DescriptionLocation = new Uri($"http://{settings.ChromecastHosts[i].Ip}") },
                                    new SsdpRootDevice { FriendlyName = settings.ChromecastHosts[i].Name }
                                );
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private void SaveSettings()
        {
            var hosts = settings.ChromecastHosts;
            if (hosts == null)
                hosts = new List<UserSettingHost>();
            foreach (var host in devices.GetHosts())
            {
                if (!hosts.Any(x => x.Ip == host.Ip))
                {
                    hosts.Add(host);
                }
            }
            settings.ChromecastHosts = hosts;
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

        public void ResetSettings()
        {
            settings.ChromecastHosts = new List<UserSettingHost>();
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

        public async void SetStreamFormat(SupportedStreamFormat format)
        {
            if (format != StreamFormatSelected)
            {
                StreamFormatSelected = format;
                Mp3Stream = null;

                playingOnIpOrFormatChange = devices.Stop();
                if (playingOnIpOrFormatChange)
                {
                    await Task.Delay(2500);
                    devices.Start();
                    await Task.Delay(15000);
                    playingOnIpOrFormatChange = false;
                }
            }
        }

        public void SetCulture(string culture)
        {
            Culture = culture;
        }
    }
}
