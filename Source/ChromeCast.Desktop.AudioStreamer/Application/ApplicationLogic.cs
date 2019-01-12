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
        private bool playingOnIpChange;
        private UserSettings settings = new UserSettings();

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
            devices.OnRecordingDataAvailable(dataToSend, format, reduceLagThreshold);
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
            var menuItem = new MenuItem();
            menuItem.Text = device.GetFriendlyName();
            menuItem.Click += device.OnClickDeviceButton;
            notifyIcon.ContextMenu.MenuItems.Add(notifyIcon.ContextMenu.MenuItems.Count - 1, menuItem);
            device.SetMenuItem(menuItem);

            mainForm.AddDevice(device);
        }

        private void AddNotifyIcon()
        {
            var contextMenu = new ContextMenu();
            var menuItem = new MenuItem();
            menuItem.Index = 0;
            menuItem.Text = "Close";
            menuItem.Click += new EventHandler(CloseApplication);
            contextMenu.MenuItems.AddRange(new MenuItem[] { menuItem });

            notifyIcon = new NotifyIcon();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            notifyIcon.Icon = ((Icon)(resources.GetObject("$this.Icon")));
            notifyIcon.Visible = true;
            notifyIcon.Text = "ChromeCast Desktop Streamer";
            notifyIcon.ContextMenu = contextMenu;
            notifyIcon.Click += ToggleFormVisibility;
        }

        public string GetStreamingUrl()
        {
            return streamingUrl;
        }

        public void SetLagThreshold(int lagThreshold)
        {
            reduceLagThreshold = lagThreshold;
        }

        public void SetConfiguration(bool showLog, bool showLagControl, int lagValue, string ipAddressesDevices)
        {
            mainForm.ShowLog(showLog);
            mainForm.ShowLagControl(showLagControl);
            mainForm.SetLagValue(lagValue);

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
            if (playingOnIpChange)
            {
                playingOnIpChange = false;
                return true;
            }
            else
            {
                return AutoRestart;
            }
        }

        public async void ChangeIPAddressUsed(IPAddress ipAddress)
        {
            playingOnIpChange = devices.Stop();
            streamingRequestListener.StopListening();
            await Task.Run(() => { streamingRequestListener.StartListening(ipAddress, OnStreamingRequestsListen, OnStreamingRequestConnect); });
            if (playingOnIpChange)
            {
                await Task.Delay(2500);
                devices.Start();
                await Task.Delay(15000);
                playingOnIpChange = false;
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
            mainForm.SetKeyboardHooks(settings.UseKeyboardShortCuts ?? false);
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
                hosts = new List<SettingHost>();
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

            settings.Save();
        }

        public void ResetSettings()
        {
            settings.ChromecastHosts = new List<SettingHost>();
            settings.UseKeyboardShortCuts = false;
            settings.AutoStartDevices = false;
            settings.ShowWindowOnStart = true;
            settings.AutoRestart = false;
            devices.SetAutoStart(settings.AutoStartDevices.Value);
            mainForm.SetAutoStart(settings.AutoStartDevices.Value);
            mainForm.SetAutoRestart(settings.AutoRestart.Value);
            mainForm.SetWindowVisibility(settings.ShowWindowOnStart.Value);
            mainForm.SetKeyboardHooks(settings.UseKeyboardShortCuts.Value);
            settings.Save();
        }
    }
}
